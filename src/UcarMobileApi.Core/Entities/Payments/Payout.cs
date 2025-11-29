using System;
using UcarMobileApi.Core.Entities.Technicians;

namespace UcarMobileApi.Core.Entities.Payments;

/// <summary>
/// Represents a payout operation made by the payments provider (e.g. Stripe) to a technician's connected account.
/// This record is used for audit, reconciliation and status tracking.
/// </summary>
public class Payout : EntityBase
{
    /// <summary>
    /// Foreign key to the Technician receiving the payout.
    /// </summary>
    public int TechnicianId { get; set; }

    /// <summary>
    /// Amount in cents of the payout.
    /// </summary>
    public long AmountCents { get; set; }

    /// <summary>
    /// Currency of the payout (ISO 4217), e.g. "usd".
    /// </summary>
    public string Currency { get; set; } = "usd";

    /// <summary>
    /// Current status reported by the provider (e.g. "paid", "in_transit", "failed").
    /// </summary>
    public string Status { get; set; } = "pending";

    /// <summary>
    /// The provider's payout identifier (e.g. Stripe payout id: po_....).
    /// </summary>
    public string? ProviderPayoutId { get; set; }

    /// <summary>
    /// Optional provider-specific destination (e.g. bank account token or destination id).
    /// </summary>
    public string? ProviderDestination { get; set; }

    /// <summary>
    /// Timestamp when the provider created the payout (UTC).
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Navigation property to the Technician.
    /// </summary>
    public Technician Technician { get; set; } = null!;
}
