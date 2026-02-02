using System;
using System.Collections.Generic;
using UcarMobileApi.Core.Entities.Clients;
using UcarMobileApi.Core.Entities.Common;
using UcarMobileApi.Core.Entities.Payments;
using UcarMobileApi.Core.Enums;

namespace UcarMobileApi.Core.Entities.Appointments;

public partial class Appointment : EntityBase
{
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;

    public ICollection<AppointmentVehicle> Vehicles { get; set; } = [];

    // Scheduling
    public DateTime ScheduledStart { get; set; }        // Client-selected date/time
    public DateTime? ScheduledEnd { get; set; }         // Optional, can be calculated
    public DateTime? CompletedAt { get; set; }

    // Service location (geofencing & radius validation)
    public AddressInfo ServiceAddress { get; set; } = null!;

    // Pricing & billing
    public decimal EstimatedTotal { get; set; }
    public decimal Tax { get; set; }

    // Payment
    public int? PaymentMethodId { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;

    // Current status: Requested > Confirmed > Assigned > EnRoute > InProgress > Completed > Closed
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Requested;

    public ICollection<AppointmentNote> Notes { get; set; } = [];
    public ICollection<AppointmentDocument> Documents { get; set; } = [];
    public ICollection<Payment> Payments { get; set; } = [];

    public int WarrantyMonths { get; set; }
    public int WarrantyMiles { get; set; }
    public string? CancellationReason { get; set; }

}

// IMPORTANT: AppointmentStatus ordering matters (Completed and above are immutable)
public enum AppointmentStatus : byte
{
    Requested = 1,         // Lead captured, not confirmed yet
    Confirmed,             // Fully locked (email + payment OK)
    EnRoute,
    InProgress,
    Completed,             // Work finished, invoice generated, Paid + review possible
    Cancelled
}
