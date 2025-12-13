using System;
using System.Collections.Generic;
using UcarMobileApi.Application.DTOs.Clients;
using UcarMobileApi.Application.DTOs.Common;

namespace UcarMobileApi.Application.DTOs.Appointments;

public class AppointmentCreateDto
{
    public int ClientId { get; set; }

    public DateTime ScheduledStart { get; set; }
    public DateTime? ScheduledEnd { get; set; }

    public AddressInfoDto ServiceAddress { get; set; } = new();

    public decimal EstimatedTotal { get; set; }

    // List of vehicles involved in the appointment
    // Each contains the full ClientVehicle info (NOT only VehicleId)
    public List<ClientVehicleCreateDto> Vehicles { get; set; } = [];
}

// DTO returned by the API when reading an Appointment
public class AppointmentDto
{
    public int Id { get; set; }

    public int ClientId { get; set; }

    public DateTime ScheduledStart { get; set; }
    public DateTime? ScheduledEnd { get; set; }

    public AddressInfoDto ServiceAddress { get; set; } = new();

    public decimal EstimatedTotal { get; set; }

    public List<AppointmentVehicleDto> Vehicles { get; set; } = [];
    public List<AppointmentNoteDto> Notes { get; set; } = [];
}

public class AppointmentVehicleDto
{
    public int Id { get; set; }
    public int VehicleId { get; set; }

    public List<AppointmentServiceDto> Services { get; set; } = [];
    public List<AppointmentPartDto> Parts { get; set; } = [];
}

public class AppointmentNoteDto
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;

    public int Source { get; set; }
}
