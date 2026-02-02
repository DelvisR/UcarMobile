using System;
using System.Collections.Generic;
using UcarMobileApi.Application.DTOs.Common;
using UcarMobileApi.Core.Entities.Appointments;
using UcarMobileApi.Core.Enums;

namespace UcarMobileApi.Application.DTOs.Appointments;

public class UpdateAppointmentRequest
{
    public AppointmentStatus? Status { get; set; }

    public DateTime? ScheduledStart { get; set; }
    public DateTime? ScheduledEnd { get; set; }
    public DateTime? CompletedAt { get; set; }

    public AddressInfoDto? ServiceAddress { get; set; }

    public int? PaymentMethodId { get; set; }
    public PaymentStatus? PaymentStatus { get; set; }

    public decimal? EstimatedTotal { get; set; }
    public string? CancellationReason { get; set; }
}

public sealed class CompleteAppointmentRequest
{
    public int? WarrantyMonths { get; init; }
    public int? WarrantyMiles { get; init; }

    /// <summary>
    /// Optional override; defaults to UtcNow if null
    /// </summary>
    public DateTime? CompletedAt { get; init; }

    /// <summary>
    /// Optional odometer updates per appointment vehicle
    /// </summary>
    public IReadOnlyCollection<CompleteAppointmentVehicleRequest> Vehicles { get; init; } = [];
}

public sealed class CompleteAppointmentVehicleRequest
{
    public int Id { get; init; }
    public int? OdometerKm { get; init; }
}

/// <summary>
/// Request for cancelling an appointment. CancellationReason is optional.
/// </summary>
public sealed class CancelAppointmentRequest
{
    /// <summary>
    /// Optional free-text reason for cancellation (max length enforced by EF mapping).
    /// </summary>
    public string? CancellationReason { get; init; }
}

public class RescheduleRequest
{
    public DateTime ScheduledStart { get; set; }
    public DateTime? ScheduledEnd { get; set; }
}
