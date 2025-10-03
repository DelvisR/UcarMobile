using System.Text.Json;

namespace UcarMobileApi.Application.Common;

/// <summary>
/// Provides preconfigured JsonSerializerOptions instances to reuse across the application.
/// </summary>
public static class JsonDefaults
{
    /// <summary>
    /// Case-insensitive options (useful for AWS Secrets Manager responses).
    /// </summary>
    public static readonly JsonSerializerOptions CaseInsensitive = new()
    {
        PropertyNameCaseInsensitive = true
    };
}
