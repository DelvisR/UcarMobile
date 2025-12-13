using UcarMobileApi.Core.Entities.Common;
using UcarMobileApi.Core.Entities.Vehicles;

namespace UcarMobileApi.Core.Entities.Services;

public class Estimate : AuditableEntity
{
    public int ServiceId { get; set; }
    public Service Service { get; set; } = null!;

    public int VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;

    public decimal LaborMaxCost { get; set; }
    public decimal LaborMinCost { get; set; }
    public decimal PartMaxCost { get; set; }
    public decimal PartMinCost { get; set; }
}
