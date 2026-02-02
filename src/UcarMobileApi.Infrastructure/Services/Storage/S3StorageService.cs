using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Options;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Application.DTOs.Files;
using UcarMobileApi.Infrastructure.Configurations.Settings;

namespace UcarMobileApi.Infrastructure.Services.Storage;

/// <summary>
/// AWS S3 implementation of IFileStorageService.
/// </summary>
public class S3StorageService(IAmazonS3 s3, IOptions<AwsSettings> options) : IStorageService
{
    private readonly AwsSettings _awsSettings = options.Value;

    public UploadPresignResponse GenerateUploadPresignedUrl(string? prefix, string fileName, string? contentType, int? expiresMinutes)
    {
        ValidateS3Config();

        var bucket = _awsSettings.S3.BucketName;
        var key = BuildKey(prefix, fileName);
        var expires = DateTime.UtcNow.AddMinutes(expiresMinutes ?? _awsSettings.S3.PresignMinutes);

        // Generate presigned PUT URL (computed locally; no network call to AWS)
        var preReq = new GetPreSignedUrlRequest
        {
            BucketName = bucket,
            Key = key,
            Verb = HttpVerb.PUT,
            Expires = expires,
            ContentType = string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType
        };

        return new UploadPresignResponse { Key = key, UploadUrl = s3.GetPreSignedURL(preReq) };
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

    public async Task<StoredFileMetadata> UploadAsync(Stream fileStream, string fileName, string contentType, string? prefix, CancellationToken ct)
    {
        ValidateS3Config();

        var bucket = _awsSettings.S3.BucketName;
        var key = BuildKey(prefix, fileName);

        var transfer = new TransferUtility(s3);
        var uploadReq = new TransferUtilityUploadRequest
        {
            BucketName = bucket,
            Key = key,
            InputStream = fileStream,
            ContentType = string.IsNullOrWhiteSpace(contentType)
                ? "application/octet-stream"
                : contentType
        };

        await transfer.UploadAsync(uploadReq, ct);

        return new StoredFileMetadata(
            bucket,
            key,
            fileName,
            uploadReq.ContentType ?? "application/octet-stream",
            TryGetStreamLength(fileStream)
        );
    }

    public async Task<StoredFileMetadata> GetObjectMetadataAsync(string key, CancellationToken cancellationToken = default)
    {
        ValidateS3Config();

        var bucket = _awsSettings.S3.BucketName;

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

        return new StoredFileMetadata(
            bucket,
            key,
            Path.GetFileName(key),
            ContentType: meta.Headers.ContentType ?? "application/octet-stream",
            meta.Headers.ContentLength
        );
    }

    /// <summary>
    /// Deletes a file from S3.
    /// </summary>
    /// <exception cref="KeyNotFoundException"></exception>
    public async Task DeleteAsync(string bucket, string key, CancellationToken cancellationToken)
    {
        // Delete from S3 (does not fail if it does not exist)
        await s3.DeleteObjectAsync(bucket, key, cancellationToken);
    }

    /// <summary>
    /// Deletes multiple objects from S3. If bucket is null or empty, uses configured default bucket.
    /// Assumes keys is a collection of valid keys; only validates keys is not null.
    /// </summary>
    public async Task DeleteObjectsAsync(string? bucket, IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keys);

        if (string.IsNullOrWhiteSpace(bucket))
        {
            ValidateS3Config();
            bucket = _awsSettings.S3.BucketName;
        }

        var deleteRequest = new DeleteObjectsRequest
        {
            BucketName = bucket,
            Objects = keys.Select(k => new KeyVersion { Key = k }).ToList()
        };

        await s3.DeleteObjectsAsync(deleteRequest, cancellationToken);
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
