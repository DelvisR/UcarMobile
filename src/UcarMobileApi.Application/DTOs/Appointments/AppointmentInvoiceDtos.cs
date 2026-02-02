using System;
using System.Collections.Generic;

namespace UcarMobileApi.Application.DTOs.Appointments;

public class AppointmentInvoiceDto
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public string ClientEmail { get; set; } = string.Empty;
    public string ClientPhone { get; set; } = string.Empty;
    public string ClientFullName { get; set; } = string.Empty;
    public DateTime ScheduledStart { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string ServiceFullAddress { get; set; } = string.Empty;
    public double ServiceAddressLat { get; set; }
    public double ServiceAddressLng { get; set; }
    public decimal EstimatedTotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Discount { get; set; }
    public List<AppointmentInvoiceVehicleDto> Vehicles { get; set; } = [];
    public int WarrantyMonths { get; set; }
    public int WarrantyMiles { get; set; }
}

public class AppointmentInvoiceVehicleDto
{
    public int OdometerKm { get; set; }
    public string VehicleName { get; set; } = string.Empty;
    public string VehicleVIN { get; set; } = string.Empty;
    public string VehicleLicense { get; set; } = string.Empty;

    public int TechnicianId { get; set; }
    public string TechnicianName { get; set; } = string.Empty;

    public List<AppointmentServiceDto> Services { get; set; } = [];
}
