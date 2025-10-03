using System.Threading.RateLimiting;

namespace UcarMobileApi.Configuration;

/// <summary>
/// Basic security middleware
/// - Rate limiter
/// - HTTP Strict Transport Security
/// - Content Security Headers
/// </summary>
public static class SecurityMiddlewareExtensions
{
    /// <summary>
    /// Register basic security services such as rate limiting
    /// </summary>
    public static IServiceCollection AddBasicSecurity(this IServiceCollection services)
    {
        // Configure Rate Limiting (example: 100 requests/min per IP)
        services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 100, // Maximum number of allowed requests
                        Window = TimeSpan.FromMinutes(1), // Time window
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 2
                    }));
        });

        return services;
    }

    /// <summary>
    /// Apply basic security middleware in the request pipeline
    /// </summary>
    public static IApplicationBuilder UseBasicSecurity(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Enable HSTS only in production environment
        if (!env.IsDevelopment())
        {
            app.UseHsts();
        }

        // Add recommended OWASP security headers
        app.Use(async (context, next) =>
        {
            // Typed header properties (strongly-typed)
            context.Response.Headers.XContentTypeOptions = "nosniff";    // Prevent MIME type sniffing
            context.Response.Headers.XFrameOptions = "DENY";             // Prevent clickjacking
            context.Response.Headers.XXSSProtection = "1; mode=block";   // Basic XSS protection (legacy)

            // Headers without typed properties (use string indexer)
            context.Response.Headers["Referrer-Policy"] = "no-referrer"; // Limit referrer information

            // Adjust Content-Security-Policy according to your frontend requirements
            // Example for Vue/Vite (to be refined later):
            // context.Response.Headers.ContentSecurityPolicy = "default-src 'self'"; // To be refined for your frontend

            await next();
        });

        // Enable Rate Limiting
        app.UseRateLimiter();

        return app;
    }
}
