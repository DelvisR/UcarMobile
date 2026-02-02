using Gridify;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Infrastructure.Configurations.Gridify;
using UcarMobileApi.Infrastructure.Configurations.Gridify.Mappers;
using UcarMobileApi.Infrastructure.Configurations.Gridify.Operators;

namespace UcarMobileApi.Configuration;

/// <summary>
/// Provides extension methods to configure Gridify globally for the application.
/// This includes setting global behaviors, registering custom operators,
/// and scanning assemblies for Gridify mappers.
/// </summary>
public static class GridifyConfiguration
{
    /// <summary>
    /// Registers and configures Gridify services and global settings.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> where services are registered.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> to allow method chaining.</returns>
    public static IServiceCollection AddGridifyConfiguration(this IServiceCollection services)
    {
        // Enable case-insensitive filtering globally
        GridifyGlobalConfiguration.CaseInsensitiveFiltering = true;

        // Enable compatibility layer for Entity Framework (translates expressions to SQL properly)
        GridifyGlobalConfiguration.EntityFrameworkCompatibilityLayer = true;

        // Register custom operators (can be used in filtering syntax)
        GridifyGlobalConfiguration.CustomOperators.Register<InOperator>();

        // Register Gridify mappers from the specified assembly
        // Gridify mappers allow creating virtual columns and custom property mappings
        services.AddGridifyMappers(typeof(GridifyMapperAssemblyMarker).Assembly);
        // Register the Gridify mapper resolver for resolving mappers at runtime
        services.AddScoped<IGridifyMapperResolver, GridifyMapperResolver>();


        return services;
    }
}
