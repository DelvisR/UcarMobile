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
    public string? Prefix { get; set; }
    public int? ExpiresMinutes { get; set; }
}

public class UploadPresignResponse
{
    public string Key { get; set; } = null!;
    public string UploadUrl { get; set; } = string.Empty;
}

/// <summary>
/// Request payload to notify the API after client finished uploading to S3.
/// </summary>
public class FileUploadCompleteDto
{
    public string Key { get; set; } = null!;
    public string? OriginalFileName { get; set; }
    public string? ContentType { get; set; }
}
