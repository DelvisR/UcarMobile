using System.Collections.Generic;

namespace UcarMobileApi.Core.Entities.ServiceZone;

public class ServiceZone : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string BaseAddress { get; set; } = string.Empty;
    public double RadiusMiles { get; set; } = 25.0;
    public double Lat { get; set; }
    public double Lng { get; set; }
    public List<string> ZipCodes { get; set; } = [];
    public bool IsActive { get; set; } = true;
}
