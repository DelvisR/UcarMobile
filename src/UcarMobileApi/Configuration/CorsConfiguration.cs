namespace UcarMobileApi.Configuration;

/// <summary>
/// Provides extension methods for configuring Cross-Origin Resource Sharing (CORS).
/// </summary>
public static class CorsConfiguration
{
    private const string DefaultPolicyName = "DefaultCorsPolicy";

    /// <summary>
    /// Adds and configures CORS services using values from appsettings.json.
    /// </summary>
    public static IServiceCollection AddCorsServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var corsSection = configuration.GetSection("Cors");

        var allowedOrigins = corsSection.GetSection("AllowedOrigins").Get<string[]>() ?? ["*"];
        var allowedMethods = corsSection.GetSection("AllowedMethods").Get<string[]>() ?? ["*"];
        var allowedHeaders = corsSection.GetSection("AllowedHeaders").Get<string[]>() ?? ["*"];
        var exposedHeaders = corsSection.GetSection("ExposedHeaders").Get<string[]>() ?? [];
        var allowCredentials = corsSection.GetValue("AllowCredentials", true);
        var maxAge = corsSection.GetValue("MaxAge", 1800);

        services.AddCors(options =>
        {
            options.AddPolicy(DefaultPolicyName, builder =>
            {
                // ORIGINS
                if (allowedOrigins is ["*"])
                {
                    builder.AllowAnyOrigin();
                    builder.DisallowCredentials();
                }
                else
                {
                    builder.WithOrigins(allowedOrigins);

                    if (allowCredentials)
                        builder.AllowCredentials();
                    else
                        builder.DisallowCredentials();
                }

                // METHODS
                if (allowedMethods is ["*"])
                    builder.AllowAnyMethod();
                else
                    builder.WithMethods(allowedMethods);

                // HEADERS
                if (allowedHeaders is ["*"])
                    builder.AllowAnyHeader();
                else
                    builder.WithHeaders(allowedHeaders);

                // EXPOSED HEADERS
                if (exposedHeaders.Length > 0)
                    builder.WithExposedHeaders(exposedHeaders);

                // MAX AGE
                if (maxAge > 0)
                    builder.SetPreflightMaxAge(TimeSpan.FromSeconds(maxAge));
            });
        });

        return services;
    }

    /// <summary>
    /// Enables the CORS middleware with the configured policy.
    /// </summary>
    public static IApplicationBuilder UseCorsConfiguration(this IApplicationBuilder app)
    {
        app.UseCors(DefaultPolicyName);
        return app;
    }
}
