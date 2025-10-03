using Newtonsoft.Json;

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
                opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
    }
}
