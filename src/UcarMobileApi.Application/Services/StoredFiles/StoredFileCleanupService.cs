using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Core.Entities.Storage;

namespace UcarMobileApi.Application.Services.StoredFiles;

public class StoredFileCleanupService(IAppDbContext db, IStorageService storage, ILogger<StoredFileCleanupService> logger) : IStoredFileCleanupService
{
    private const int BatchSize = 1000;

    public async Task RunCleanupAsync(CancellationToken cancellationToken)
    {
        // Loop until there are no more files to process or cancellation is requested
        while (!cancellationToken.IsCancellationRequested)
        {
            // Step 1: Atomically claim a batch of files using row-level locks
            var files = await ClaimFilesAsync(cancellationToken);

            if (files.Count == 0)
            {
                // Nothing to clean
                return;
            }

            // Step 2: Group files by bucket to optimize S3 batch deletion
            var buckets = files.GroupBy(f => f.Bucket);

            foreach (var bucketGroup in buckets)
            {
                try
                {
                    // Step 3: Perform batch delete in S3 (idempotent operation)
                    await storage.DeleteObjectsAsync(bucketGroup.Key, bucketGroup.Select(f => f.Key), cancellationToken);
                }
                catch (Exception ex)
                {
                    // If S3 delete fails, revert the state so another worker can retry
                    logger.LogError(ex, "Failed to delete objects from bucket {Bucket}", bucketGroup.Key);

                    await RevertToPendingAsync(bucketGroup, cancellationToken);
                    continue;
                }
            }

            // Step 4: Remove successfully deleted files from database
            db.Set<StoredFile>().RemoveRange(files);
            await db.SaveChangesAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Claims a batch of files for deletion using PostgreSQL row-level locking.
    /// This guarantees that multiple instances will never process the same row.
    /// </summary>
    private async Task<List<StoredFile>> ClaimFilesAsync(CancellationToken ct)
    {
        // Using raw SQL because EF Core does not support FOR UPDATE SKIP LOCKED natively
        var files = await db.Set<StoredFile>()
            .FromSqlRaw(@"
                SELECT *
                FROM ""StoredFile""
                WHERE ""DeleteStatus"" = {0}
                ORDER BY ""Id""
                LIMIT {1}
                FOR UPDATE SKIP LOCKED",
                DeleteStatus.Pending,
                BatchSize)
            .ToListAsync(ct);

        if (files.Count == 0)
            return files;

        // Mark rows as Processing and release the lock as fast as possible
        foreach (var file in files)
        {
            file.DeleteStatus = DeleteStatus.Processing;
        }

        await db.SaveChangesAsync(ct);

        return files;
    }

    /// <summary>
    /// Reverts files back to Pending state if S3 deletion fails.
    /// This allows future cleanup attempts.
    /// </summary>
    private async Task RevertToPendingAsync(IEnumerable<StoredFile> files, CancellationToken ct)
    {
        foreach (var file in files)
        {
            file.DeleteStatus = DeleteStatus.Pending;
        }

        await db.SaveChangesAsync(ct);
    }
}
