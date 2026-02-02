using UcarMobileApi.Core.Entities.Common;

namespace UcarMobileApi.Core.Entities.Storage;

/// <summary>
/// File metadata stored in the database to mirror S3 objects.
/// Ensures synchronization between DB and S3.
/// </summary>
public class StoredFile : EntityBase
{
    /// <summary>S3 bucket name.</summary>
    public string Bucket { get; set; } = string.Empty;

    /// <summary>Full S3 object key (path inside bucket).</summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>Original file name provided by client.</summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>File content type (MIME).</summary>
    public string ContentType { get; set; } = "application/octet-stream";

    /// <summary>Size in bytes.</summary>
    public long Size { get; set; }

    public DeleteStatus DeleteStatus { get; set; }
}

public enum DeleteStatus : byte
{
    None = 0,
    Pending = 1,
    Processing = 2
}
