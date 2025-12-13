using System.Collections.Generic;
using UcarMobileApi.Core.Entities.Common;

namespace UcarMobileApi.Core.Entities.Vehicles;

public class VehicleSubModel : EntityBase
{
    public int VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;

    public string Name { get; set; } = string.Empty; // 60 character max

    public int? AzId { get; set; }
    public ICollection<VehicleEngine> Engines { get; set; } = [];

}
