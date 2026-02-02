using System;
using System.Collections.Generic;
using UcarMobileApi.Application.DTOs.Clients;
using UcarMobileApi.Application.DTOs.Common;
using UcarMobileApi.Application.DTOs.Payments;
using UcarMobileApi.Application.DTOs.Technicians;
using UcarMobileApi.Core.Entities.Appointments;

namespace UcarMobileApi.Application.DTOs.Appointments;

public class AppointmentDto
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public DateTime ScheduledStart { get; set; }
    public DateTime ScheduledEnd { get; set; }
    public DateTime? CompletedAt { get; set; }
    public AddressInfoDto ServiceAddress { get; set; } = null!;
    public decimal EstimatedTotal { get; set; }
    public decimal Tax { get; set; }

    public IReadOnlyCollection<AppointmentDiscountDto> Discounts { get; set; } = [];
    public PaymentMethodDto? PaymentMethod { get; set; }
    public AppointmentStatus Status { get; set; }
    public List<AppointmentVehicleDto> Vehicles { get; set; } = [];
    public int? WarrantyMonths { get; set; }
    public string? CancellationReason { get; set; }
    public ICollection<AppointmentNoteBaseDto> Notes { get; set; } = [];
}

public class AppointmentVehicleDto
{
    public int Id { get; set; }
    public ClientVehicleBasicDto Vehicle { get; set; } = null!;

    public int OdometerKm { get; set; }

    public TechnicianBasicDto? Technician { get; set; }

    public List<AppointmentServiceDto> Services { get; set; } = [];
}
