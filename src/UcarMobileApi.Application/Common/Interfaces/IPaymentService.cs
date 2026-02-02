using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UcarMobileApi.Application.DTOs.Payments;

namespace UcarMobileApi.Application.Common.Interfaces;

/// <summary>
/// Defines the contract for payment-related operations such as setup, method storage,
/// charge creation, and refunds. This abstraction allows using different payment providers.
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// Returns all payment methods for a client.
    /// </summary>
    Task<IEnumerable<PaymentMethodListDto>> GetPaymentMethodsAsync(string authProviderId, CancellationToken cancellationToken = default);
    Task<IEnumerable<PaymentMethodListDto>> GetPaymentMethodsByClientIdAsync(int clientId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Initializes a payment setup process for a client.
    /// This allows the client to register one or more payment methods without being charged.
    /// </summary>
    Task<PaymentSetupDto> CreateSetupIntentAsync(string authProviderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Attaches a payment method to a client and saves it in the local database.
    /// </summary>
    Task<PaymentMethodDto> AttachPaymentMethodAsync(string authProviderId, PaymentMethodAttachDto dto, CancellationToken cancellationToken = default);

    Task DetachPaymentMethodAsync(string authProviderId, int paymentMethodId, CancellationToken cancellationToken = default);

    Task SetDefaultPaymentMethodAsync(string authProviderId, int paymentMethodId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates and confirms a payment for a specific client.
    /// </summary>
    Task<PaymentDto> CreatePaymentAsync(string authProviderId, PaymentCreateDto dto, CancellationToken cancellationToken = default);
    Task<PaymentDto> CreatePaymentByClientIdAsync(int clientId, PaymentCreateDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Issues a refund for an existing payment, either partial or full.
    /// </summary>
    Task<PaymentRefundResultDto> RefundPaymentAsync(PaymentRefundDto dto, CancellationToken cancellationToken = default);

    // === Payouts === /

    /// <summary>
    /// Creates a provider (connect) account for a given technician and returns an onboarding URL.
    /// This will persist the provider account id to the technician entity.
    /// </summary>
    Task<OnboardResponseDto> CreateAccountAsync(string authProviderId, OnboardRequestDto dto, CancellationToken cancellationToken);

    /// <summary>
    /// Creates a fresh account link for an existing provider account to allow onboarding refresh.
    /// </summary>
    Task<OnboardResponseDto> RefreshOnboardingLinkAsync(string authProviderId, OnboardRequestDto dto, CancellationToken cancellationToken);

    /// <summary>
    /// Transfers funds from the platform balance to the technician's connected account.
    /// </summary>
    Task<PayoutDto> MakeTransferAsync(string authProviderId, PayoutCreateDto createDto, CancellationToken cancellationToken);
}
