using System.Collections.Generic;
using UcarMobileApi.Core.Entities.Clients;
using UcarMobileApi.Core.Entities.Common;

namespace UcarMobileApi.Core.Entities.Payments;

/// <summary>
/// Represents a stored payment method belonging to a specific client.
/// Usually corresponds to a Stripe payment method (e.g., card).
/// </summary>
public class PaymentMethod : EntityBase
{
    /// <summary>
    /// The client who owns this payment method.
    /// </summary>
    public int ClientId { get; set; }

    /// <summary>
    /// The unique identifier of the payment method in the external provider (e.g., Stripe "pm_xxx").
    /// </summary>
    public string ProviderPaymentMethodId { get; set; } = string.Empty;

    /// <summary>
    /// The brand of the card (e.g., Visa, MasterCard, Amex).
    /// </summary>
    public string Brand { get; set; } = string.Empty;

    /// <summary>
    /// The last four digits of the card number.
    /// </summary>
    public string Last4 { get; set; } = string.Empty;

    /// <summary>
    /// The card’s expiration month (1–12).
    /// </summary>
    public int ExpMonth { get; set; }

    /// <summary>
    /// The card’s expiration year (e.g., 2028).
    /// </summary>
    public int ExpYear { get; set; }

    /// <summary>
    /// Indicates whether this payment method is the default for the client.
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// Navigation property to the associated client.
    /// </summary>
    public Client Client { get; set; } = null!;

    public IEnumerable<Payment> Payments { get; set; } = [];
}
