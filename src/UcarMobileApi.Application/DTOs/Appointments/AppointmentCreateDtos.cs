using System;
using System.Collections.Generic;
using UcarMobileApi.Application.Attributes;
using UcarMobileApi.Application.DTOs.Clients;
using UcarMobileApi.Application.DTOs.Common;
using UcarMobileApi.Core.Entities.Appointments;

namespace UcarMobileApi.Application.DTOs.Appointments;

public class AppointmentCreateDto
{
    public int ClientId { get; set; }

    public DateTime ScheduledStart { get; set; }

    [SwaggerIgnore]
    public DateTime? ScheduledEnd { get; set; }
    public AddressInfoDto ServiceAddress { get; set; } = new();

    public decimal EstimatedTotal { get; set; }

    public long? PaymentMethodId { get; set; }
    public bool IsContactCenter { get; set; } = false;

    [SwaggerIgnore]
    public AppointmentStatus Status { get; set; }

    public List<AppointmentVehicleCreateDto> Vehicles { get; set; } = [];
    public List<AppointmentNoteBaseDto> Notes { get; set; } = [];
}

public class AppointmentVehicleCreateDto
{
    [SwaggerIgnore]
    public int? TechnicianId { get; set; }
    public ClientVehicleUpsertDto Vehicle { get; set; } = new();
    public ICollection<AppointmentServiceCreateDto> Services { get; set; } = [];
}
