using Microsoft.OpenApi.Models;
using System.Reflection;

namespace UcarMobileApi.Configuration;

/// <summary>
/// Provides extension methods to configure Swagger/OpenAPI documentation.
/// </summary>
/// <remarks>
/// Sets up Swagger UI and OpenAPI generation for the API project.
/// </remarks>
public static class SwaggerConfiguration
{
    /// <summary>
    /// Adds and configures Swagger/OpenAPI documentation services.
    /// </summary>
    /// <param name="services">The service collection to register Swagger services in.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "UcarMobile API",
                Version = "v1",
                Description = "API for UcarMobile with AWS Cognito Authentication"
            });

            // Add Security Definition for Bearer token
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer' followed by your valid Cognito JWT token.\nExample: Bearer eyJhbGciOi..."
            });

            // Add Security Requirement
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    []
                }
            });

            // **Enable XML comments**
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });
        return services;
    }

    /// <summary>
    /// Adds Swagger and Swagger UI to the application's request pipeline.
    /// </summary>
    /// <param name="app">The application builder used to configure middleware.</param>
    /// <returns>The updated application builder with Swagger enabled.</returns>
    /// <remarks>
    /// Enables the Swagger JSON endpoint and the Swagger UI for API exploration and testing.
    /// </remarks>
    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        return app;
    }
}