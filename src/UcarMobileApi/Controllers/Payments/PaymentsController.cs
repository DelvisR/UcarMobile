using Microsoft.AspNetCore.Mvc;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Application.DTOs.Payments;
using UcarMobileApi.Application.Services.Users;
using UcarMobileApi.Authorization;

namespace UcarMobileApi.Controllers.Payments;

/// <summary>
/// Handles HTTP endpoints related to payment setup, charges, and refunds.
/// </summary>
[ApiController]
[Route("api/payments")]
public class PaymentsController(IPaymentService paymentService, CurrentUserService currentUser) : ControllerBase
{
    /// <summary>
    /// Returns all saved payment methods for the current authenticated user.
    /// Requires 'ACTION_VIEW_PAYMENT_METHODS' action.
    /// </summary>
    [HttpGet("methods")]
    [RequireAction("ACTION_VIEW_PAYMENT_METHODS")]
    [ProducesResponseType(typeof(IEnumerable<PaymentMethodListDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PaymentMethodListDto>>> GetPaymentMethods(CancellationToken cancellationToken)
    {
        var result = await paymentService.GetPaymentMethodsAsync(currentUser.AuthProviderId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Returns all saved payment methods for a client.
    /// Requires 'ACTION_VIEW_PAYMENT_METHODS' action.
    /// </summary>
    [HttpGet("clients/{clientId:int}/methods")]
    [RequireAction("ACTION_VIEW_PAYMENT_METHODS")]
    [ProducesResponseType(typeof(IEnumerable<PaymentMethodListDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PaymentMethodListDto>>> GetPaymentMethodsByClient(int clientId, CancellationToken cancellationToken)
    {
        var result = await paymentService.GetPaymentMethodsByClientIdAsync(clientId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Initializes a payment setup process for a specific client.
    /// Allows registering a card without charging it.
    /// Requires 'ACTION_MANAGE_PAYMENTS' action.
    /// </summary>
    [HttpPost("setup-intent")]
    [RequireAction("ACTION_MANAGE_PAYMENTS")]
    [ProducesResponseType(typeof(PaymentSetupDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaymentSetupDto>> CreateSetupIntent(CancellationToken cancellationToken)
    {
        var result = await paymentService.CreateSetupIntentAsync(currentUser.AuthProviderId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Attaches a payment method to a client and persists it locally.
    /// Requires 'ACTION_MANAGE_PAYMENTS' action.
    /// </summary>
    [HttpPost("attach-method")]
    [RequireAction("ACTION_MANAGE_PAYMENTS")]
    [ProducesResponseType(typeof(PaymentMethodDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaymentMethodDto>> AttachPaymentMethod([FromBody] PaymentMethodAttachDto dto, CancellationToken cancellationToken)
    {
        var result = await paymentService.AttachPaymentMethodAsync(currentUser.AuthProviderId, dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Detaches a payment method from a client and marks it as deleted locally.
    /// Requires 'ACTION_MANAGE_PAYMENTS' action.
    /// </summary>
    [HttpDelete("methods/{paymentMethodId}")]
    [RequireAction("ACTION_MANAGE_PAYMENTS")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DetachPaymentMethod(int paymentMethodId, CancellationToken cancellationToken)
    {
        await paymentService.DetachPaymentMethodAsync(currentUser.AuthProviderId, paymentMethodId, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Sets a payment method as the default for the current user.
    /// Requires 'ACTION_MANAGE_PAYMENTS' action.
    /// </summary>
    [HttpPost("methods/{id:int}/set-default")]
    [RequireAction("ACTION_MANAGE_PAYMENTS")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SetDefaultPaymentMethod(int id, CancellationToken cancellationToken)
    {
        await paymentService.SetDefaultPaymentMethodAsync(currentUser.AuthProviderId, id, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Creates and confirms a payment for the given client.
    /// Requires 'ACTION_CREATE_PAYMENT' action.
    /// </summary>
    [HttpPost("charge")]
    [RequireAction("ACTION_CREATE_PAYMENT")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaymentDto>> CreatePayment([FromBody] PaymentCreateDto dto, CancellationToken cancellationToken)
    {
        var result = await paymentService.CreatePaymentAsync(currentUser.AuthProviderId, dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Creates and confirms a payment for a specific client.
    /// Requires 'ACTION_CREATE_PAYMENT' action.
    /// </summary>
    [HttpPost("clients/{clientId:int}/charge")]
    [RequireAction("ACTION_CREATE_PAYMENT")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaymentDto>> CreatePaymentByClient(int clientId, [FromBody] PaymentCreateDto dto, CancellationToken cancellationToken)
    {
        var result = await paymentService.CreatePaymentByClientIdAsync(clientId, dto, cancellationToken);

        return Ok(result);
    }


    /// <summary>
    /// Issues a refund for a specific payment.
    /// Requires 'ACTION_REFUND_PAYMENT' action.
    /// </summary>
    [HttpPost("refund")]
    [RequireAction("ACTION_REFUND_PAYMENT")]
    [ProducesResponseType(typeof(PaymentRefundResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaymentRefundResultDto>> Refund([FromBody] PaymentRefundDto dto, CancellationToken cancellationToken)
    {
        var result = await paymentService.RefundPaymentAsync(dto, cancellationToken);
        return Ok(result);
    }

    // ================= PAYOUTS ============== //

    /// <summary>
    /// Create a provider onboarding account and return an onboarding url.
    /// </summary>
    [HttpPost("payout/onboard")]
    [RequireAction("ACTION_MANAGE_PAYOUTS")]
    [ProducesResponseType(typeof(OnboardResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<OnboardResponseDto>> Onboard([FromBody] OnboardRequestDto dto, CancellationToken cancellationToken)
    {
        var result = await paymentService.CreateAccountAsync(currentUser.AuthProviderId, dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Generate a fresh onboarding link for an existing provider account (if previous link expired).
    /// </summary>
    [HttpPost("payout/onboard/refresh")]
    [RequireAction("ACTION_MANAGE_PAYOUTS")]
    [ProducesResponseType(typeof(OnboardResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<OnboardResponseDto>> RefreshOnboardLink([FromBody] OnboardRequestDto dto, CancellationToken cancellationToken)
    {
        var result = await paymentService.RefreshOnboardingLinkAsync(currentUser.AuthProviderId, dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Issues a payout (transfer) to a connected account.
    /// </summary>
    [HttpPost("payout/transfer")]
    [RequireAction("ACTION_CREATE_PAYOUTS")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<PayoutDto>> Payout([FromBody] PayoutCreateDto createDto, CancellationToken cancellationToken)
    {
        var result = await paymentService.MakeTransferAsync(currentUser.AuthProviderId, createDto, cancellationToken);
        return Ok(result);
    }
}
