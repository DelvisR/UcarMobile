namespace UcarMobileApi.Application.Common;

/// <summary>
/// Holds the Cognito Root User ID for the lifetime of the application.
/// </summary>
public class CognitoRootUserOptions
{
    public string UserRootCognitoId { get; set; } = string.Empty;
}
