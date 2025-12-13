using System.Collections.Generic;
using UcarMobileApi.Core.Entities.Common;
using UcarMobileApi.Core.Entities.Technicians;

namespace UcarMobileApi.Core.Entities.Services;

public class ServiceZone : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string BaseAddress { get; set; } = string.Empty;
    public double RadiusMiles { get; set; } = 25.0;
    public double Lat { get; set; }
    public double Lng { get; set; }
    public List<string> ZipCodes { get; set; } = [];
    public bool IsActive { get; set; } = true;

    public ICollection<TechnicianServiceZone> Technicians { get; set; } = [];
}
