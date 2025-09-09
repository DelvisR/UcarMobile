using UcarMobileApi.Infrastructure.Configurations.Settings;

namespace UcarMobileApi.Configuration;

/// <summary>
/// Provides extension methods for registering strongly typed application settings.
/// </summary>
public static class SettingsConfiguration
{
    /// <summary>
    /// Registers application settings (AWS and other AppSetting config) from configuration.
    /// </summary>
    /// <param name="services">The service collection where settings will be registered.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddAppSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AwsSettings>().Bind(configuration.GetSection("AWS")).ValidateDataAnnotations().ValidateOnStart();

        return services;
    }
}