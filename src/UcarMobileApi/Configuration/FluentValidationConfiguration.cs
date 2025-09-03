using FluentValidation;
using UcarMobileApi.Application;

namespace UcarMobileApi.Configuration;

public static class FluentValidationConfiguration
{
    public static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(ApplicationAssemblyMarker).Assembly);
        return services;
    }
}