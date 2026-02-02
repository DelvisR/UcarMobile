using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UcarMobileApi.Infrastructure.Configurations.AutoZone.Models;
using UcarMobileApi.Infrastructure.Factories;
using UcarMobileApi.Infrastructure.Providers;
using UcarMobileApi.Infrastructure.Services.AutoZone.Interfaces;
using UcarMobileApi.Infrastructure.Configurations.AutoZone;

namespace UcarMobileApi.Infrastructure.Services.AutoZone.Extensions
{
    public static class AutoZoneServiceCollectionExtensions
    {
        public static IServiceCollection AddAutoZone(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Settings (single source of truth)
            services.AddSingleton<AutoZoneSettingsDto>();

            // Initializer
            services.AddSingleton<AutoZoneSettingsFactory>();

            // Providers
            services.AddScoped<IAutoZoneTokenProvider, AutoZoneTokenProvider>();
            services.AddScoped<IAutoZoneService, AutoZoneService>();

            services.AddAutoZoneSettings<
                IAutoZoneHttpClientProvider,
                AutoZoneHttpClientProvider>(configuration);

            return services;
        }
    }
}
