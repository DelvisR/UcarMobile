using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Converters;

namespace UcarMobileApi.Configuration;

/// <summary>
/// Provides extension methods to configure MVC options.
/// </summary>
/// <remarks>
/// Configures JSON serialization, controllers, and other MVC-specific options.
/// </remarks>
public static class MvcConfiguration
{
    /// <summary>
    /// Adds MVC configuration with custom JSON serialization settings and controller options.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The updated service collection.</returns>
    public static IMvcBuilder AddMvcConfiguration(this IServiceCollection services)
    {
        return services.AddControllers()
            .AddNewtonsoftJson(opt =>
            {
				// Prevent reference loops (common with EF navigation properties)
                opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
				// Do not include null properties in JSON
                opt.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

				// Serialize/deserialize enums as strings
                opt.SerializerSettings.Converters.Add(new StringEnumConverter());

                // Add StoredFileDto converter
                opt.SerializerSettings.Converters.Add(new StoredFileDtoNewtonsoftConverter(
                    services.BuildServiceProvider().GetRequiredService<IStorageService>()
                ));
            });
    }

}
