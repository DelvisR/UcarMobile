namespace UcarMobileApi.Application.DTOs.Files;

/// <summary>
/// Data transfer object representing a stored file with an optional presigned URL.
/// </summary>
public class StoredFileDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }

    /// <summary>
    /// Temporary URL for downloading the file from S3 (presigned, expires soon).
    /// </summary>
    public string? Url { get; set; }
}
