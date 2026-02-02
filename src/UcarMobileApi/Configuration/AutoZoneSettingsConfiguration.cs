using UcarMobileApi.Infrastructure.Factories;

namespace UcarMobileApi.Configuration
{
    /// <summary>
    /// Configuration file to get AutoZone settings from AWS Secrets Manager or local configuration.
    /// </summary>
    public static class AutoZoneSettingsConfiguration
    {
        /// <summary>
        /// Initializes AutoZone API configuration using the cached secret key.
        /// </summary>
        public static async Task InitializeAutoZone(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var autoZoneFactory =
                scope.ServiceProvider.GetRequiredService<AutoZoneSettingsFactory>();

            await autoZoneFactory.ConfigureAutoZoneSettings();
        }

    }
}
