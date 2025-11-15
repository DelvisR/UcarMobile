using System.Threading;
using System.Threading.Tasks;
using UcarMobileApi.Application.DTOs;

namespace UcarMobileApi.Application.Common.Interfaces;

/// <summary>
/// Defines the contract for payment-related operations such as setup, method storage,
/// charge creation, and refunds. This abstraction allows using different payment providers.
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// Initializes a payment setup process for a client.
    /// This allows the client to register one or more payment methods without being charged.
    /// </summary>
    Task<PaymentSetupDto> CreateSetupIntentAsync(int clientId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists a confirmed payment method from the client into the database.
    /// </summary>
    Task<PaymentMethodDto> SavePaymentMethodAsync(PaymentMethodCreateDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Attaches a payment method to a client and saves it in the local database.
    /// </summary>
    Task<PaymentMethodDto> AttachPaymentMethodAsync(PaymentMethodAttachDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates and confirms a payment for a specific client.
    /// </summary>
    Task<PaymentDto> CreatePaymentAsync(PaymentCreateDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Issues a refund for an existing payment, either partial or full.
    /// </summary>
    Task<PaymentRefundResultDto> RefundPaymentAsync(PaymentRefundDto dto, CancellationToken cancellationToken = default);
}
