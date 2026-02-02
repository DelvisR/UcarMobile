using FluentValidation;
using UcarMobileApi.Application;
using UcarMobileApi.Application.Validators.Common;

namespace UcarMobileApi.Configuration;

/// <summary>
/// Provides extension methods to configure FluentValidation services.
/// </summary>
/// <remarks>
/// Registers validators and integrates them with ASP.NET Core's model validation pipeline.
/// </remarks>
public static class FluentValidationConfiguration
{
    /// <summary>
    /// Adds FluentValidation configuration and registers all validators found in the assembly.
    /// </summary>
    /// <param name="services">The service collection to register validators in.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(ApplicationAssemblyMarker).Assembly);
        // register the validator resolver
        services.AddScoped<IValidatorResolver, FluentValidatorFactory>();
        return services;
    }
}
