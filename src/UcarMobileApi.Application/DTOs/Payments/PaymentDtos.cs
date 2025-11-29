namespace UcarMobileApi.Application.DTOs.Payments;

/// <summary>
/// Represents the response of a SetupIntent creation.
/// </summary>
public record PaymentSetupDto(string ClientSecret, string ProviderCustomerId);

/// <summary>
/// DTO used to attach a payment method to a client.
/// </summary>
public record PaymentMethodAttachDto(string ProviderPaymentMethodId, bool IsDefault);

/// <summary>
/// DTO used to create and confirm a payment intent.
/// </summary>
public record PaymentCreateDto(int PaymentMethodId, string IdempotencyKey, long AmountCents, string Currency = "usd");

/// <summary>
/// DTO used to request a refund.
/// </summary>
public record PaymentRefundDto(int PaymentId, long? AmountCents, string IdempotencyKey); // AmountCents = null => total refund

/// <summary>
/// Represents the result of a refund operation.
/// </summary>
/// <param name="ProviderRefundId">Provider refund identifier (e.g. Stripe refund id).</param>
/// <param name="Status">Refund status from the provider (e.g. succeeded, pending).</param>
/// <param name="AmountCents">Amount refunded in cents.</param>
/// <param name="Currency">Currency code (e.g. "usd").</param>
public record PaymentRefundResultDto(string ProviderRefundId, string Status, long? AmountCents, string Currency)
{
    /// <summary>
    /// Error code returned by the payment provider, used for frontend message localization.
    /// </summary>
    public string? ErrorCode { get; init; }
};

/// <summary>
/// Represents a saved payment method in the system.
/// </summary>
public record PaymentMethodDto(int Id, string Brand, string Last4, int ExpMonth, int ExpYear, bool IsDefault);

/// <summary>
/// For list payment methods.
/// </summary>
public record PaymentMethodListDto(int Id, string Brand, string Last4, bool IsDefault);


/// <summary>
/// Represents a processed payment operation.
/// </summary>
public record PaymentDto(string ProviderPaymentId, string Status, long AmountCents, string Currency)
{
    /// <summary>
    /// Optional client secret for payments requiring 3D Secure or further authentication.
    /// </summary>
    public string? ClientSecret { get; init; }

    /// <summary>
    /// Error code returned by the payment provider, used for frontend message localization.
    /// </summary>
    public string? ErrorCode { get; init; }
}


