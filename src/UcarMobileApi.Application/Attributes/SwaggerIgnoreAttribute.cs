using System;

namespace UcarMobileApi.Application.Attributes;

/// <summary>
/// Attribute to mark properties that should be hidden in Swagger documentation.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class SwaggerIgnoreAttribute : Attribute
{
}
