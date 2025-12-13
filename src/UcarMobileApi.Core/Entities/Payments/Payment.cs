using System.Collections.Generic;
using UcarMobileApi.Core.Entities.Appointments;
using UcarMobileApi.Core.Entities.Clients;
using UcarMobileApi.Core.Entities.Common;

namespace UcarMobileApi.Core.Entities.Payments;

/// <summary>
/// Represents a payment transaction made by a client through a payment provider (e.g., Stripe).
/// </summary>
public class Payment : EntityBase
{
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
    public int PaymentMethodId { get; set; }
    public PaymentMethod PaymentMethod { get; set; } = null!;
    public string ProviderPaymentId { get; set; } = string.Empty; // payment_intent id
    public Dictionary<string, string> MetadataJson { get; set; } = new();// traceability
    public long AmountCents { get; set; }
    public string Currency { get; set; } = "usd";
    public string Status { get; set; } = "pending";
    public string? ClientSecret { get; init; }
    public string? ErrorCode { get; set; }

    // Optional relationship with Appointment (when payment is for an appointment)
    public int? AppointmentId { get; set; }
    public Appointment? Appointment { get; set; }

    /// <summary>
    /// Collection of refunds related to this payment.
    /// </summary>
    public ICollection<PaymentRefund> Refunds { get; set; } = [];
}
