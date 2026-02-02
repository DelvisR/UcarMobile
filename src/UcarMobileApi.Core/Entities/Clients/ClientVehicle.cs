using System.Collections.Generic;
using UcarMobileApi.Core.Entities.Appointments;
using UcarMobileApi.Core.Entities.Common;
using UcarMobileApi.Core.Entities.Vehicles;

namespace UcarMobileApi.Core.Entities.Clients;

public class ClientVehicle : EntityBase
{
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
    public int VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;

    // Vehicle Details
    public string? VIN { get; set; }
    public string? LicensePlate { get; set; }
    public string? Submodel { get; set; }
    public string? Engine { get; set; }
    public string? VehicleType { get; set; }
    public string? BodyType { get; set; }
    public string? Fuel { get; set; }
    public int OdometerKm { get; set; }
    public string? Notes { get; set; }
    public int? AzId { get; set; }

    public string FullName => $"{Vehicle.Year} {Vehicle.Make} {Submodel ?? Vehicle.Model} {Engine}".Trim();

    public ICollection<AppointmentVehicle> Appointments { get; set; } = [];
}
