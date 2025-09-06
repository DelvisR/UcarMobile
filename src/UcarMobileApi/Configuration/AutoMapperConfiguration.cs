using AutoMapper.EquivalencyExpression;
using UcarMobileApi.Application;

namespace UcarMobileApi.Configuration;

/// <summary>
/// Provides extension methods to configure AutoMapper profiles.
/// </summary>
/// <remarks>
/// Registers mapping profiles from the Application layer for dependency injection.
/// </remarks>
public static class AutoMapperConfiguration
{
    /// <summary>
    /// Adds AutoMapper profiles to the service collection.
    /// </summary>
    /// <param name="services">The service collection to register profiles in.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddAutoMapperProfiles(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg =>
        {
            cfg.AddCollectionMappers(); // Enable AutoMapper Collection
        }, typeof(ApplicationAssemblyMarker).Assembly);
        return services;
    }
}