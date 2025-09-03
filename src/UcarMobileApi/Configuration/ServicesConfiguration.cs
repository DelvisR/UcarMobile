using UcarMobileApi.Application.Services;

namespace UcarMobileApi.Configuration;

public static class ServicesConfiguration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssemblyOf<ServiceRegistrationMarker>() // Assembly reference
            .AddClasses(classes => classes.InNamespaceOf<ServiceRegistrationMarker>()) // only this namespace
            .AsSelf() // without interfaces
            .AsImplementedInterfaces()// with interfaces 
            .WithScopedLifetime()
        );

        // Register IHttpContextAccessor
        services.AddHttpContextAccessor();

        return services;
    }
}