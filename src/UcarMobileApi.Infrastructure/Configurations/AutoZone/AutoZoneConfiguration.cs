using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UcarMobileApi.Infrastructure.Configurations.AutoZone.Models;

namespace UcarMobileApi.Infrastructure.Configurations.AutoZone
{
    /// <summary>
    /// Provides configuration settings and utilities for the AutoZone api integration.
    /// </summary>
    public static class AutoZoneConfiguration
    {
        /// <summary>
        /// Adds and configures an HTTP client for AutoZone API access using settings bound from the specified
        /// configuration.
        /// </summary>
        /// <remarks>This method binds AutoZone-specific settings from the provided configuration and
        /// configures the HTTP client with those settings. The registered client uses a custom HTTP message handler
        /// that disables automatic redirects and enables default credentials. Ensure that the required AutoZoneSettings
        /// are registered in the service collection before calling this method.</remarks>
        /// <typeparam name="TClient">The interface or base type for the HTTP client to register. Must be a class.</typeparam>
        /// <typeparam name="TImplementation">The concrete implementation of <typeparamref name="TClient"/> to use for the HTTP client. Must be a class
        /// and implement <typeparamref name="TClient"/>.</typeparam>
        /// <param name="services">The service collection to which the HTTP client and related settings will be added.</param>
        /// <param name="configuration">The configuration source containing AutoZone API settings, such as the base URL.</param>
        public static void AddAutoZoneSettings<TClient, TImplementation>(this IServiceCollection services, IConfiguration configuration) where TClient : class where TImplementation : class, TClient
        {
            //  Bind AutoZone settings from configuration
            services.AddHttpClient<TClient, TImplementation>((sp, c) =>
            {
                var autoZoneSettings = sp.GetRequiredService<AutoZoneSettingsDto>();
                c.BaseAddress = new Uri(autoZoneSettings.ApiBaseUrl);
            })
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    return new HttpClientHandler
                    {
                        AllowAutoRedirect = false, // Custom setting
                        UseDefaultCredentials = true
                    };
                });


        }


    }
}
