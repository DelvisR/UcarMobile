using UcarMobileApi.Middleware;

namespace UcarMobileApi.Configuration;

/// <summary>
/// Provides extension methods for configuring custom middleware.
/// </summary>
/// <remarks>
/// Registers middleware components that are specific to the application.
/// </remarks>
public static class MiddlewareConfiguration
{
    /// <summary>
    /// Configures the HTTP request pipeline with custom middleware components.
    /// </summary>
    /// <param name="app">The application builder used to configure middleware.</param>
    /// <returns>The updated application builder.</returns>
    public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder app)
    {
        app.UseExceptionHandling();
        return app;
    }
}
