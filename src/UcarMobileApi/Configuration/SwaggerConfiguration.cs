using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using UcarMobileApi.Filters;

namespace UcarMobileApi.Configuration;

/// <summary>
/// Provides extension methods to configure Swagger/OpenAPI documentation.
/// </summary>
/// <remarks>
/// Sets up Swagger UI and OpenAPI generation for the API project.
/// Automatically handles JWT Bearer authentication and respects [AllowAnonymous] endpoints.
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
            // Basic OpenAPI info
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

            // Display enums as strings
            c.UseInlineDefinitionsForEnums();

            // Remove global security requirement, we'll add it per endpoint via filter
            // c.AddSecurityRequirement(...) removed

            // Apply OperationFilter to automatically add security only to protected endpoints
            c.OperationFilter<AuthResponsesOperationFilter>();

            c.SchemaFilter<EnumAsStringSchemaFilter>();

            // XML Comments: register ALL projects that generate XML
            var basePath = AppContext.BaseDirectory;

            foreach (var file in Directory.GetFiles(basePath, "*.xml", SearchOption.TopDirectoryOnly))
            {
                c.IncludeXmlComments(file, true);
            }

            // Custom filter to ignore specific properties from Swagger
            c.SchemaFilter<SwaggerIgnoreFilter>();
        });

        services.AddSwaggerGenNewtonsoftSupport();

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

        app.UseSwaggerUI(c =>
        {
            // Sort endpoint
            c.ConfigObject.AdditionalItems["tagsSorter"] = "alpha";

            // Sort HTTP methods
            c.ConfigObject.AdditionalItems["operationsSorter"] = "method";
        });

        return app;
    }
}

/// <summary>
/// Swagger operation filter to apply security only to endpoints that require authentication.
/// </summary>
public class AuthResponsesOperationFilter : IOperationFilter
{
    /// <summary>
    /// Applies security requirements to Swagger operations based on authorization attributes.
    /// Only endpoints without [AllowAnonymous] will have the JWT Bearer lock icon.
    /// </summary>
    /// <param name="operation">The OpenAPI operation to modify.</param>
    /// <param name="context">Context providing method and type metadata.</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Check if [AllowAnonymous] is applied on the controller or method safely
        var hasAnonymous = (context.MethodInfo.DeclaringType?.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any() ?? false)
                           || (context.MethodInfo.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any());

        if (hasAnonymous)
        {
            // Public endpoint, do not add security requirement
            return;
        }

        // Protected endpoint: add JWT Bearer security
        operation.Security = new List<OpenApiSecurityRequirement>
        {
            new OpenApiSecurityRequirement
            {
                [ new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    }
                ] = Array.Empty<string>()
            }
        };
    }
}

/// <summary>
/// Swagger schema filter that forces enum types to be represented as strings
/// instead of numeric values.
/// 
/// This improves API usability and documentation by exposing enum names
/// (e.g. "Android", "iOS") rather than their underlying numeric values (e.g. 1, 2).
/// 
/// Note:
/// This filter affects Swagger/OpenAPI documentation only and does not alter
/// JSON serialization behavior at runtime.
/// </summary>
public sealed class EnumAsStringSchemaFilter : ISchemaFilter
{
    /// <summary>
    /// Applies the schema transformation for enum types.
    /// 
    /// When the target type is an enum, the schema is modified to:
    /// - Use "string" as the OpenAPI type
    /// - Remove numeric enum values
    /// - Populate the enum list with the enum member names
    /// </summary>
    /// <param name="schema">The OpenAPI schema being generated.</param>
    /// <param name="context">Contextual information about the schema generation.</param>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var type = context.Type;

        if (!type.IsEnum)
            return;

        schema.Type = "string";
        schema.Format = null;

        schema.Enum.Clear();

        foreach (var name in Enum.GetNames(type))
        {
            schema.Enum.Add(new OpenApiString(name));
        }
    }
}
