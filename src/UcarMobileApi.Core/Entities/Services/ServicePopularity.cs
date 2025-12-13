using UcarMobileApi.Core.Entities.Common;
using UcarMobileApi.Core.Entities.Vehicles;

namespace UcarMobileApi.Core.Entities.Services;

public class ServicePopularity : AuditableEntity
{
    public int ServiceId { get; set; }
    public Service Service { get; set; } = null!;

    public int VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;

    public int Count { get; set; }
}
