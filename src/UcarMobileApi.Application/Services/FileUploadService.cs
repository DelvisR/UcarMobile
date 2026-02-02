using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using UcarMobileApi.Application.Common.Interfaces;

namespace UcarMobileApi.Application.Services;

public class FileUploadService(IStorageService storage)
{
    public async Task<IReadOnlyCollection<StoredFileMetadata>> UploadFilesAsync(IReadOnlyCollection<IFormFile> files, string prefix, CancellationToken ct)
    {
        var results = new ConcurrentBag<StoredFileMetadata>();

        await Parallel.ForEachAsync(files, new ParallelOptions { MaxDegreeOfParallelism = 4, CancellationToken = ct },
            async (file, token) =>
            {
                await using var stream = file.OpenReadStream();
                var metadata = await storage.UploadAsync(stream, file.FileName, file.ContentType, prefix, token);

                results.Add(metadata);
            });

        return [.. results];
    }

    public async Task CleanupUploadedFilesAsync(IEnumerable<StoredFileMetadata> uploaded, CancellationToken ct)
    {
        foreach (var file in uploaded)
        {
            try
            {
                await storage.DeleteAsync(file.Bucket, file.Key, ct);
            }
            catch
            {
                // TODO: log error
            }
        }
    }
}
