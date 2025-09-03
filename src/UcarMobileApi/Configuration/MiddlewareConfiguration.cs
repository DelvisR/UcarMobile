using UcarMobileApi.Middleware;

namespace UcarMobileApi.Configuration;

public static class MiddlewareConfiguration
{
    public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder app)
    {
        app.UseExceptionHandling();
        return app;
    }
}