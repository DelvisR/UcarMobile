using System.Collections.Generic;
using UcarMobileApi.Application.DTOs.Appointments;

namespace UcarMobileApi.Application.DTOs.Clients;

// Represents a ClientVehicle being sent when creating an Appointment.
// This object may or may not exist in the DB.
public class ClientVehicleCreateDto
{
    public int VehicleId { get; set; }   // Vehicle exists in DB

    // Optional: could be deduced from Appointment.ClientId
    public int? ClientId { get; set; }

    // Vehicle Details
    public string? VIN { get; set; }
    public string? LicensePlate { get; set; }
    public string? Submodel { get; set; }
    public string? Engine { get; set; }
    public string? VehicleType { get; set; }
    public string? BodyType { get; set; }
    public string? Fuel { get; set; }
    public string? Notes { get; set; }

    public List<AppointmentServiceCreateDto> Services { get; set; } = [];
    public List<AppointmentPartCreateDto> Parts { get; set; } = [];
}
