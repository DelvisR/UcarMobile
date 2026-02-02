using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UcarMobileApi.Application.DTOs.Files;

namespace UcarMobileApi.Application.Common.Interfaces;

/// <summary>
/// File storage abstraction for upload, presign flows and metadata sync.
/// </summary>
public interface IStorageService
{
    // Presign for client direct upload
    UploadPresignResponse GenerateUploadPresignedUrl(string? prefix, string fileName, string? contentType, int? expiresMinutes);

    // Presign download by S3 key (legacy/compat)
    string GenerateDownloadPresignedUrl(string key, string? bucket = null, int? expiresMinutes = null);

    // Server-side upload
    Task<StoredFileMetadata> UploadAsync(Stream fileStream, string fileName, string contentType, string? prefix, CancellationToken cancellationToken = default);

    // Notify DB after client direct upload
    Task<StoredFileMetadata> GetObjectMetadataAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a file from S3.
    /// </summary>
    Task DeleteAsync(string bucket, string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete multiple objects from a bucket by keys. If bucket is null or empty,
    /// the infrastructure layer should use the configured default bucket.
    /// The method returns no value and assumes keys contain valid entries.
    /// </summary>
    Task DeleteObjectsAsync(string? bucket, IEnumerable<string> keys, CancellationToken cancellationToken = default);
}

/// <summary>
/// Metadata returned after uploading to S3 (not yet persisted).
/// </summary>
public record StoredFileMetadata(string Bucket, string Key, string FileName, string ContentType, long Size);
