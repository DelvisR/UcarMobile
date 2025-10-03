using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Extensions.Caching; // <-- contains SecretsManagerCache & SecretCacheConfiguration
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Infrastructure.Configurations.Settings;
using UcarMobileApi.Infrastructure.Services;

namespace UcarMobileApi.Configuration
{
    /// <summary>
    /// Extension methods for registering AWS Secrets Manager client, caching, and secret provider.
    /// </summary>
    public static class SecretsManagerConfiguration
    {
        /// <summary>
        /// Registers AWS Secrets Manager client, cache, and the custom secret provider.
        /// Uses the <see cref="AwsSettings.Secrets"/> to configure cache TTL.
        /// </summary>
        public static IServiceCollection AddAwsSecretsManager(this IServiceCollection services, AwsSettings awsSettings)
        {
            // 1) Create the AmazonSecretsManager client using configured region.
            services.AddSingleton<IAmazonSecretsManager>(_ =>
                new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(awsSettings.Region)));

            // 2) Configure the secret cache. Note: SecretCacheConfiguration.CacheItemTTL is in milliseconds.
            services.AddSingleton<ISecretsManagerCache>(sp =>
            {
                var client = sp.GetRequiredService<IAmazonSecretsManager>();

                // Convert minutes to milliseconds (uint)
                var minutes = awsSettings.Secrets?.CacheMinutes ?? 15;
                var ttlMs = (uint)TimeSpan.FromMinutes(minutes).TotalMilliseconds;

                var cacheConfig = new SecretCacheConfiguration
                {
                    CacheItemTTL = ttlMs,
                    MaxCacheSize = 1024,      // optional: keep default or change
                    VersionStage = "AWSCURRENT"
                };

                // Construct cache with the provided client and configuration
                return new SecretsManagerCache(client, cacheConfig);
            });

            // 3) Register your provider that wraps the cache.
            services.AddSingleton<ISecretProvider, SecretProvider>();

            return services;
        }
    }
}
