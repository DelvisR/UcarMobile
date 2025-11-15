using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UcarMobileApi.Application.DTOs.Files;
using UcarMobileApi.Core.Entities.Storage;

namespace UcarMobileApi.Application.Common.Interfaces;

/// <summary>
/// File storage abstraction for upload, presign flows and metadata sync.
/// </summary>
public interface IFileStorageService
{
    // Presign download by S3 key (legacy/compat)
    string GenerateDownloadPresignedUrl(string key, string? bucket = null, int? expiresMinutes = null);

    // Server-side upload
    Task<StoredFile> UploadAsync(Stream fileStream, string fileName, string contentType, string? prefix, CancellationToken cancellationToken = default);

    // Presign for client direct upload
    Task<string> GenerateUploadPresignedUrl(UploadPresignRequest request);

    // Notify DB after client direct upload
    Task<StoredFile> NotifyUploadAsync(NotifyUploadRequest request, CancellationToken cancellationToken = default);

    // Presign download by StoredFile Id
    Task<string> GenerateDownloadPresignedUrlByIdAsync(int id, int? expiresMinutes, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a file from S3 and removes its record from the database.
    /// </summary>
    /// <param name="id">Stored file ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True if deleted, false if not found.</returns>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
