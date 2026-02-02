using System.Collections.Generic;
using UcarMobileApi.Core.Entities.Clients;
using UcarMobileApi.Core.Entities.Common;
using UcarMobileApi.Core.Entities.Services;

namespace UcarMobileApi.Core.Entities.Vehicles;

public class Vehicle : EntityBase
{
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }

    public string FullName => $"{Year} {Make} {Model}".Trim();

    public ICollection<VehicleSubModel> SubModels { get; set; } = [];

    public ICollection<ClientVehicle> Vehicles { get; set; } = [];
    public ICollection<Estimate> Estimates { get; set; } = [];
    public ICollection<ServicePopularity> PopularServices { get; set; } = [];

}
