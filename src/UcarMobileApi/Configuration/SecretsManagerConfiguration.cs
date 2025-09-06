using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Extensions.Caching;
using UcarMobileApi.Infrastructure.Configurations.Settings;
using UcarMobileApi.Infrastructure.Services;

namespace UcarMobileApi.Configuration;

/// <summary>
/// Provides extension methods to register AWS Secrets Manager services.
/// </summary>
public static class SecretsManagerConfiguration
{
    /// <summary>
    /// Registers AWS Secrets Manager client and cache in the DI container.
    /// </summary>
    /// <param name="services">The service collection where dependencies are registered.</param>
    /// <param name="awsSettings">The AWS settings containing region configuration.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddAwsSecretsManager(this IServiceCollection services, AwsSettings awsSettings)
    {
        // Register the Amazon Secrets Manager client
        services.AddSingleton<IAmazonSecretsManager>(_ =>
            new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(awsSettings.Region)));

        // Register the Secrets Manager cache
        services.AddSingleton<ISecretsManagerCache>(sp =>
        {
            var client = sp.GetRequiredService<IAmazonSecretsManager>();
            return new SecretsManagerCache(client);
        });

        // Register SecretProvider that implements the Application interface
        services.AddSingleton<ISecretProvider, SecretProvider>();

        return services;
    }
}