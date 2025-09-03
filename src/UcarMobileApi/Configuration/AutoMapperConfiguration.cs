using UcarMobileApi.Application;

namespace UcarMobileApi.Configuration;

public static class AutoMapperConfiguration
{
    public static IServiceCollection AddAutoMapperProfiles(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ApplicationAssemblyMarker).Assembly);
        return services;
    }
}