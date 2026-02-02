using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using UcarMobileApi.Application.Attributes;

namespace UcarMobileApi.Filters;

/// <summary>
/// Removes properties marked with [SwaggerIgnore] from Swagger schema.
/// </summary>
public class SwaggerIgnoreFilter : ISchemaFilter
{
    /// <inheritdoc/>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema?.Properties == null || context?.Type == null)
            return;

        // Get properties with custom attribute
        var excludedProperties = context.Type.GetProperties()
            .Where(p => p.GetCustomAttributes(typeof(SwaggerIgnoreAttribute), true).Any())
            .Select(p => p.Name);

        foreach (var excludedProperty in excludedProperties)
        {
            // Remove from schema (case-insensitive)
            var propertyToRemove = schema.Properties.Keys
                .FirstOrDefault(k => string.Equals(k, excludedProperty, StringComparison.OrdinalIgnoreCase));

            if (propertyToRemove != null)
            {
                schema.Properties.Remove(propertyToRemove);
            }
        }
    }
}


