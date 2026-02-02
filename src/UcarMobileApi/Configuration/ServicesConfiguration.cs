using UcarMobileApi.Application.Services;
using UcarMobileApi.Infrastructure.Services.Storage;

namespace UcarMobileApi.Configuration;

/// <summary>
/// Provides extension methods to register application services.
/// </summary>
/// <remarks>
/// Uses assembly scanning to automatically register services and their interfaces with dependency injection.
/// </remarks>
public static class ServicesConfiguration
{
    /// <summary>
    /// Registers application-layer services and their dependencies into the service collection.
    /// </summary>
    /// <param name="services">The service collection to register services in.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Application Service Registration
        services.Scan(scan => scan
            .FromAssemblyOf<ServiceRegistrationMarker>() // Assembly reference
            .AddClasses(classes => classes.InNamespaceOf<ServiceRegistrationMarker>()) // only this namespace
            .AsSelf() // without interfaces
            .AsImplementedInterfaces()// with interfaces 
            .WithScopedLifetime()
        );

        // Register IHttpContextAccessor
        services.AddHttpContextAccessor();

        // Register your BackgroundService in the .NET dependency container so that it runs automatically in parallel
        // when the API is launched. Then, is Singleton by default
        services.AddHostedService<StoredFileCleanupBackgroundService>();

        return services;
    }
}
