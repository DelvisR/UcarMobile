using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Application.DTOs.Files;
using UcarMobileApi.Core.Entities.Storage;

namespace UcarMobileApi.Application.Services.StoredFiles;

public class StoredFileService(IStorageService storage, IAppDbContext context, IMapper mapper, ILogger<StoredFileService> logger)
{
    public async Task<StoredFile> UploadAndSaveAsync(Stream fileStream, string fileName, string contentType, string? prefix, CancellationToken ct = default)
    {
        // 1. Upload to S3
        var metadata = await storage.UploadAsync(fileStream, fileName, contentType, prefix, ct);

        // 2. Create entity
        var storedFile = mapper.Map<StoredFile>(metadata);

        // 3. Persist to database with rollback on failure
        context.Set<StoredFile>().Add(storedFile);

        try
        {
            await context.SaveChangesAsync(ct);
            logger.LogInformation("File saved: {Key}", storedFile.Key);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database save failed, rolling back S3 upload: {Key}", metadata.Key);
            await storage.DeleteAsync(metadata.Bucket, metadata.Key, ct);
            throw;
        }

        return storedFile;
    }

    /// <summary>
    /// Builds a <see cref="StoredFile"/> entity from a completed presigned upload.
    /// Validates that the object exists in object storage and reads its metadata,
    /// but does NOT persist anything to the database.
    /// </summary>
    /// <param name="dto">
    /// Upload completion data containing the object storage key and optional overrides
    /// such as original file name and content type.
    /// </param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// A fully populated <see cref="StoredFile"/> entity ready to be persisted
    /// as part of a larger transaction.
    /// </returns>
    /// <exception cref="FileNotFoundException">
    /// Thrown when the referenced object does not exist in object storage.
    /// </exception>
    /// <remarks>
    /// This method has no side effects and does not perform any persistence.
    /// It is intended to be used by application services that control transaction
    /// boundaries and compensation logic.
    /// </remarks>
    public async Task<StoredFile> BuildStoredFileAsync(FileUploadCompleteDto dto, CancellationToken ct = default)
    {
        // 1. Validate object exists in storage
        var metadata = await storage.GetObjectMetadataAsync(dto.Key, ct);

        // 2. Normalize values
        var fileName = !string.IsNullOrWhiteSpace(dto.OriginalFileName)
            ? dto.OriginalFileName!
            : Path.GetFileName(metadata.Key);

        var contentType = !string.IsNullOrWhiteSpace(dto.ContentType)
            ? dto.ContentType!
            : metadata.ContentType;

        // 3. Materialize entity (NO persistence)
        return new StoredFile
        {
            Bucket = metadata.Bucket,
            Key = metadata.Key,
            FileName = fileName,
            ContentType = contentType,
            Size = metadata.Size
        };
    }

    /// <summary>
    /// Registers a completed presigned upload by validating the object in storage
    /// and persisting its metadata to the database.
    /// </summary>
    /// <param name="dto">
    /// Upload completion data containing the object storage key and optional metadata overrides.
    /// </param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// The persisted <see cref="StoredFile"/> entity.
    /// </returns>
    /// <exception cref="FileNotFoundException">
    /// Thrown when the referenced object does not exist in object storage.
    /// </exception>
    /// <exception cref="Exception">
    /// Thrown when the database operation fails. In this case, the uploaded object
    /// is deleted from storage as a compensating action.
    /// </exception>
    /// <remarks>
    /// This method owns the persistence of the <see cref="StoredFile"/> entity.
    /// If the database save fails, the corresponding object in storage is deleted
    /// to prevent orphaned files.
    /// </remarks>
    public async Task<StoredFile> RegisterUploadedFileAsync(FileUploadCompleteDto dto, CancellationToken ct = default)
    {
        var storedFile = await BuildStoredFileAsync(dto, ct);

        context.Set<StoredFile>().Add(storedFile);

        try
        {
            await context.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Failed to persist uploaded file metadata. Rolling back S3 object: {Key}",
                storedFile.Key
            );

            await storage.DeleteAsync(storedFile.Bucket, storedFile.Key, ct);
            throw;
        }

        return storedFile;
    }

    public async Task<bool> DeleteAsync(int storedFileId, CancellationToken ct = default)
    {
        var entity = await context.Set<StoredFile>().FindAsync([storedFileId], ct);

        if (entity == null)
            return false;

        // Delete from S3 first (idempotent)
        await storage.DeleteAsync(entity.Bucket, entity.Key, ct);

        // Then remove from database
        context.Set<StoredFile>().Remove(entity);
        await context.SaveChangesAsync(ct);

        logger.LogInformation("File deleted: {Key}", entity.Key);
        return true;
    }

    public UploadPresignResponse GenerateUploadPresignedUrl(UploadPresignRequest request)
    {
        return storage.GenerateUploadPresignedUrl(request.Prefix, request.FileName, request.ContentType, request.ExpiresMinutes);
    }

    /// <summary>
    /// Generate presigned download URL given a StoredFile Id.
    /// Uses bucket/key from DB, supporting scenarios where caller only has the DB Id.
    /// </summary>
    public async Task<string> GenerateDownloadPresignedUrlByIdAsync(int id, int? expiresMinutes, CancellationToken cancellationToken = default)
    {
        var entity = await context.Set<StoredFile>().FindAsync([id], cancellationToken)
                     ?? throw new KeyNotFoundException("StoredFile not found.");

        var url = storage.GenerateDownloadPresignedUrl(entity.Key, entity.Bucket, expiresMinutes);

        return url;
    }
}
