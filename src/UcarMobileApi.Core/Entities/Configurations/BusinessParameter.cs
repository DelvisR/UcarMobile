using UcarMobileApi.Core.Entities.Common;

namespace UcarMobileApi.Core.Entities.Configurations;

public class BusinessParameter : EntityBase
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string ValueType { get; set; } = string.Empty; // "decimal", "bool", "string"
    public string Description { get; set; } = string.Empty;
}
