using Microsoft.AspNetCore.Http;

namespace UcarMobileApi.Application.DTOs.Files;

public class FileUploadRequest
{
    /// <summary>
    /// File to upload.
    /// </summary>
    public IFormFile File { get; set; } = null!;

    /// <summary>
    /// Optional prefix or folder path.
    /// </summary>
    public string? Prefix { get; set; }
}


/// <summary>
/// Request to generate a presigned URL for direct client upload to S3.
/// </summary>
public class UploadPresignRequest
{
    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = "application/octet-stream";
    public long? ContentLength { get; set; }
    public string? Prefix { get; set; }
    public int? ExpiresMinutes { get; set; }
}

/// <summary>
/// Request payload to notify the API after client finished uploading to S3.
/// </summary>
public class NotifyUploadRequest
{
    public string Key { get; set; } = null!;
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
}
