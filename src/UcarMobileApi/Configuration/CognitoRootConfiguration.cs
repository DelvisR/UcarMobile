using System.Text.Json;
using UcarMobileApi.Application.Common;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Infrastructure.Configurations.Settings;

namespace UcarMobileApi.Configuration;

/// <summary>
/// Provides extension methods to register the Cognito Root User Id
/// (first tries appsettings, then AWS Secrets Manager).
/// </summary>
public static class CognitoRootConfiguration
{
    /// <summary>
    /// Registers the CognitoRootUserOptions singleton in the DI container.
    /// </summary>
    /// <param name="services">The DI container.</param>
    /// <param name="configuration">The app configuration.</param>
    /// <param name="awsSettings">AWS settings.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddCognitoRootUser(this IServiceCollection services, IConfiguration configuration, AwsSettings awsSettings)
    {
        // Build a temporary provider to resolve ISecretProvider
        using var scope = services.BuildServiceProvider();
        var secretProvider = scope.GetRequiredService<ISecretProvider>();

        // 1) Try from appsettings
        var userRootId = configuration["UserRootCognitoId"];

        // 2) If not, try AWS Secrets Manager
        if (string.IsNullOrEmpty(userRootId))
        {
            var secretId = awsSettings.Secrets?.CognitoRootSecretId;

            if (!string.IsNullOrEmpty(secretId))
            {
                var secretJson = secretProvider.GetSecretAsync(secretId).GetAwaiter().GetResult();
                if (!string.IsNullOrEmpty(secretJson))
                {
                    using var doc = JsonDocument.Parse(secretJson);
                    if (doc.RootElement.TryGetProperty("UserRootCognitoId", out var prop))
                    {
                        userRootId = prop.GetString();
                    }
                }
            }
        }

        // 3) Register as singleton
        services.AddSingleton(new CognitoRootUserOptions
        {
            UserRootCognitoId = userRootId ?? string.Empty
        });

        return services;
    }
}
