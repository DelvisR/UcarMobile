using System;
using UcarMobileApi.Application.DTOs.Vehicles;

namespace UcarMobileApi.Application.DTOs.Clients;

public abstract class ClientVehicleBaseDto
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public string? VIN { get; set; }
    public string? LicensePlate { get; set; }
    public string? Submodel { get; set; }
    public string? Engine { get; set; }
    public string? VehicleType { get; set; }
    public string? BodyType { get; set; }
    public string? Fuel { get; set; }
    public string? Notes { get; set; }
    public int? AzId { get; set; }
}

// Represents a ClientVehicle being sent when creating an Appointment.
// This object may or may not exist in the DB.
public class ClientVehicleUpsertDto : ClientVehicleBaseDto;

public class ClientVehicleUpdateDto : ClientVehicleBaseDto
{
    public new int? VehicleId { get; set; }
    public int? OdometerKm { get; set; }
}

public class ClientVehicleDto
{
    public int Id { get; set; }
    public int ClientId { get; set; }

    public VehicleDto Vehicle { get; set; } = null!;

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
    public DateTime? LastServiceDate { get; set; }
}

public class ClientVehicleBasicDto
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
}
