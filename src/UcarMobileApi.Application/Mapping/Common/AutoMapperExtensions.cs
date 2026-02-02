using System;
using System.Linq;
using System.Reflection;
using AutoMapper;

namespace UcarMobileApi.Application.Mapping.Common;

public static class AutoMapperPatchExtensions
{
    /// <summary>
    /// Adds PreCondition to all simple nullable or reference properties.
    /// Ignores properties explicitly excluded (e.g., Value Objects).
    /// Safe for PATCH updates.
    /// </summary>
    public static IMappingExpression<TSource, TDestination>
        IgnoreNullValuesForPatch<TSource, TDestination>(this IMappingExpression<TSource, TDestination> mapping, params string[] ignoreProperties)
    {
        var sourceType = typeof(TSource);
        var destType = typeof(TDestination);

        foreach (var property in sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (ignoreProperties.Contains(property.Name)) continue;

            var destProp = destType.GetProperty(property.Name);
            if (destProp == null || !destProp.CanWrite) continue;

            var propType = property.PropertyType;
            if (!IsNullableOrReferenceType(propType)) continue;

            mapping.ForMember(destProp.Name, opt =>
            {
                opt.Condition(src => property.GetValue(src) != null);
            });
        }

        return mapping;
    }

    private static bool IsNullableOrReferenceType(Type type)
    {
        return !type.IsValueType || Nullable.GetUnderlyingType(type) != null;
    }
}
