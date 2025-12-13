using UcarMobileApi.Core.Entities.Common;

namespace UcarMobileApi.Core.Entities.Vehicles;

public class VehicleEngine : EntityBase
{
    public int SubModelId { get; set; }
    public VehicleSubModel SubModel { get; set; } = null!;

    public string Engine { get; set; } = string.Empty;
    public string? VehicleType { get; set; }
    public string? BodyType { get; set; }
    public string? Fuel { get; set; }
    public int AzId { get; set; }
}
