using System;
using System.Collections.Generic;
using UcarMobileApi.Core.Entities.Common;

namespace UcarMobileApi.Core.Entities.Configurations;

public class BusinessParameter : EntityBase
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string ValueType { get; set; } = string.Empty; // "decimal", "bool", "string", "int", "long", "double", "datetime", "datetimeoffset"
    public string Description { get; set; } = string.Empty;
    public int Order { get; set; }
}

public static class BusinessParameterTypes
{
    public static readonly IReadOnlyDictionary<string, Type> Map = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
    {
        ["string"] = typeof(string),
        ["int"] = typeof(int),
        ["long"] = typeof(long),
        ["decimal"] = typeof(decimal),
        ["double"] = typeof(double),
        ["bool"] = typeof(bool),
        ["datetime"] = typeof(DateTime),
        ["datetimeoffset"] = typeof(DateTimeOffset),
        ["guid"] = typeof(Guid),
        ["timespan"] = typeof(TimeSpan),
        ["phone"] = typeof(string),
        ["email"] = typeof(string),
        ["web"] = typeof(string)
    };
}
