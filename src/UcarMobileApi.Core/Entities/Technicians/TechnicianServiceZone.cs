using UcarMobileApi.Core.Entities.Common;
using UcarMobileApi.Core.Entities.Services;

namespace UcarMobileApi.Core.Entities.Technicians;

public class TechnicianServiceZone : AuditableEntity
{
    public int TechnicianId { get; set; }
    public Technician Technician { get; set; } = null!;

    public int ServiceZoneId { get; set; }
    public ServiceZone ServiceZone { get; set; } = null!;

    public bool IsPrimaryZone { get; set; }
}
