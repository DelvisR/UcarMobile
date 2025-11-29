using System;
using System.Collections.Generic;
using UcarMobileApi.Core.Entities.Clients;
using UcarMobileApi.Core.Entities.Payments;
using UcarMobileApi.Core.Enums;

namespace UcarMobileApi.Core.Entities.Appointments;

public class Appointment : EntityBase
{
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;

    public ICollection<AppointmentVehicle> Vehicles { get; set; } = [];

    // Scheduling
    public DateTime ScheduledStart { get; set; }        // Client-selected date/time
    public DateTime? ScheduledEnd { get; set; }         // Optional, can be calculated

    // Service location (geofencing & radius validation)
    public string ServiceAddress { get; set; } = string.Empty;
    public double Lat { get; set; }
    public double Lng { get; set; }

    // Pricing & billing
    public decimal EstimatedTotal { get; set; }
    // Payment Status (Separated from workflow status for better querying)
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;

    // Current status
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Requested;

    public ICollection<AppointmentNote> Notes { get; set; } = [];
    public ICollection<AppointmentDocument> Documents { get; set; } = [];
    public ICollection<Payment> Payments { get; set; } = [];
}

public enum AppointmentStatus : byte
{
    Requested = 1,         // Lead captured, not confirmed yet
    PendingVerification,   // Email verification link sent
    PendingPaymentMethod,  // Email verified, waiting for card
    PendingConfirmation,   // Agent-scheduled, waiting for client actions
    Confirmed,             // Fully locked (email + payment OK)
    Assigned,              // Technician assigned (manual or auto)
    EnRoute,
    InProgress,
    Completed,             // Work finished, invoice generated
    Closed,                // Paid + review possible
    Cancelled,
    Expired                // Pending states that timed out
}
