using Newtonsoft.Json;

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

    [JsonIgnore]
    public string? Key { get; set; } // only for internal use
    [JsonIgnore]
    public string? Bucket { get; set; } // only for internal use

    /// <summary>
    /// Temporary URL for downloading the file from S3 (presigned, expires soon).
    /// </summary>
    public string? Url { get; set; }
}
