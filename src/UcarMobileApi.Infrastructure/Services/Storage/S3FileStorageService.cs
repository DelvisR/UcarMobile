using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Options;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Application.DTOs.Files;
using UcarMobileApi.Core.Entities.Storage;
using UcarMobileApi.Infrastructure.Configurations.Settings;

namespace UcarMobileApi.Infrastructure.Services.Storage;

/// <summary>
/// AWS S3 implementation of IFileStorageService.
/// </summary>
public class S3FileStorageService(IAmazonS3 s3, IAppDbContext db, IOptions<AwsSettings> options) : IFileStorageService
{
    private readonly AwsSettings _awsSettings = options.Value;
    /// <summary>
    /// Uploads a file stream to S3 using TransferUtility (multipart-aware), then persists metadata in DB.
    /// Ensures synchronization: if DB save fails, the S3 object is deleted to avoid orphans.
    /// </summary>
    public async Task<StoredFile> UploadAsync(Stream fileStream, string fileName, string contentType, string? prefix, CancellationToken cancellationToken = default)
    {
        ValidateS3Config();

        var bucket = _awsSettings.S3.BucketName;
        var key = BuildKey(prefix, fileName);

        // Use TransferUtility for robust uploads (handles multipart, retries, large streams).
        var transfer = new TransferUtility(s3);
        var uploadReq = new TransferUtilityUploadRequest
        {
            BucketName = bucket,
            Key = key,
            InputStream = fileStream,
            ContentType = string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType
        };

        await transfer.UploadAsync(uploadReq, cancellationToken);

        var derivedSize = TryGetStreamLength(fileStream);

        var stored = new StoredFile
        {
            Bucket = bucket,
            Key = key,
            FileName = fileName,
            ContentType = uploadReq.ContentType ?? "application/octet-stream",
            Size = derivedSize
        };

        // Persist metadata; ensure synchronization on failure (compensate by deleting S3 object).
        db.Set<StoredFile>().Add(stored);
        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            await s3.DeleteObjectAsync(bucket, key, cancellationToken);
            throw;
        }

        return stored;
    }

    public Task<string> GenerateUploadPresignedUrl(UploadPresignRequest request)
    {
        ValidateS3Config();

        var bucket = _awsSettings.S3.BucketName;
        var key = BuildKey(request.Prefix, request.FileName);
        var expires = DateTime.UtcNow.AddMinutes(request.ExpiresMinutes ?? _awsSettings.S3.PresignMinutes);

        // Generate presigned PUT URL (computed locally; no network call to AWS)
        var preReq = new GetPreSignedUrlRequest
        {
            BucketName = bucket,
            Key = key,
            Verb = HttpVerb.PUT,
            Expires = expires,
            ContentType = string.IsNullOrWhiteSpace(request.ContentType) ? "application/octet-stream" : request.ContentType
        };

        var url = s3.GetPreSignedURL(preReq);

        return Task.FromResult(url);
    }

    public async Task<StoredFile> NotifyUploadAsync(NotifyUploadRequest request, CancellationToken cancellationToken = default)
    {
        ValidateS3Config();

        var bucket = _awsSettings.S3.BucketName;
        var key = request.Key;

        // Validate object exists and get its metadata (single AWS call for notify flow).
        var metaReq = new GetObjectMetadataRequest
        {
            BucketName = bucket,
            Key = key
        };

        GetObjectMetadataResponse meta;
        try
        {
            meta = await s3.GetObjectMetadataAsync(metaReq, cancellationToken);
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            throw new InvalidOperationException("Uploaded object not found in S3. Ensure the client uploaded before notifying.");
        }

        var fileName = string.IsNullOrWhiteSpace(request.FileName)
            ? Path.GetFileName(key)
            : request.FileName!;

        var contentType = string.IsNullOrWhiteSpace(request.ContentType)
            ? meta.Headers.ContentType ?? "application/octet-stream"
            : request.ContentType!;

        var stored = new StoredFile
        {
            Bucket = bucket,
            Key = key,
            FileName = fileName,
            ContentType = contentType,
            Size = meta.Headers.ContentLength
        };

        db.Set<StoredFile>().Add(stored);
        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            // DB failed; clean S3 to avoid orphan objects
            await s3.DeleteObjectAsync(bucket, key, cancellationToken);
            throw;
        }

        return stored;
    }

    public string GenerateDownloadPresignedUrl(string key, string? bucket = null, int? expiresMinutes = null)
    {
        if (bucket == null)
        {
            ValidateS3Config();

            bucket = _awsSettings.S3.BucketName;
        }

        var expires = DateTime.UtcNow.AddMinutes(expiresMinutes ?? _awsSettings.S3.PresignMinutes);

        var preReq = new GetPreSignedUrlRequest
        {
            BucketName = bucket,
            Key = key,
            Verb = HttpVerb.GET,
            Expires = expires
        };

        var url = s3.GetPreSignedURL(preReq);

        return url;
    }

    /// <summary>
    /// Generate presigned download URL given a StoredFile Id.
    /// Uses bucket/key from DB, supporting scenarios where caller only has the DB Id.
    /// </summary>
    public async Task<string> GenerateDownloadPresignedUrlByIdAsync(int id, int? expiresMinutes, CancellationToken cancellationToken = default)
    {
        var entity = await db.Set<StoredFile>().FindAsync([id], cancellationToken)
                     ?? throw new KeyNotFoundException("StoredFile not found.");

        var url = GenerateDownloadPresignedUrl(entity.Key, entity.Bucket, expiresMinutes);

        return url;
    }


    /// <summary>
    /// Deletes a file from S3 and removes its record from the database.
    /// </summary>
    /// <exception cref="KeyNotFoundException"></exception>
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await db.Set<StoredFile>().FindAsync([id], cancellationToken);

        if (entity == null)
            return false;

        // Delete from S3 (does not fail if it does not exist)
        await s3.DeleteObjectAsync(entity.Bucket, entity.Key, cancellationToken);

        // Delete the record from the database
        db.Set<StoredFile>().Remove(entity);
        await db.SaveChangesAsync(cancellationToken);

        return true;
    }


    /// <summary>
    /// Builds a normalized S3 object key without date folders.
    /// Example: basePath/prefix/{guid}-{originalFileName}.
    /// </summary>
    /// <param name="prefix">Optional prefix to categorize files (e.g., "invoices" or "avatars").</param>
    /// <param name="fileName">Original file name.</param>
    /// <returns>Normalized S3 key.</returns>
    private string BuildKey(string? prefix, string fileName)
    {
        var basePath = _awsSettings.S3.BasePath.Trim('/');
        var safeFileName = Path.GetFileName(fileName); // Removes any directory components
        var guid = Guid.NewGuid().ToString("n");

        var segments = new List<string>();
        if (!string.IsNullOrWhiteSpace(basePath))
            segments.Add(basePath);
        if (!string.IsNullOrWhiteSpace(prefix))
            segments.Add(prefix.Trim('/'));

        // Combine into key: basePath/prefix/{guid}-{fileName}
        segments.Add($"{guid}-{safeFileName}");

        return string.Join("/", segments);
    }


    private void ValidateS3Config()
    {
        if (string.IsNullOrWhiteSpace(_awsSettings.S3.BucketName))
        {
            throw new InvalidOperationException("Missing S3 bucket configuration. Set AWS:S3:BucketName in appsettings or secrets.");
        }
    }

    private static long TryGetStreamLength(Stream stream)
    {
        try
        {
            return stream.CanSeek ? stream.Length : 0L;
        }
        catch
        {
            return 0L;
        }
    }
}
