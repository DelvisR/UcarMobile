using Newtonsoft.Json;

namespace UcarMobileApi.Configuration;

public static class MvcConfiguration
{
    public static IMvcBuilder AddMvcConfiguration(this IServiceCollection services)
    {
        return services.AddControllers()
            .AddNewtonsoftJson(opt =>
            {
                opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
    }
}