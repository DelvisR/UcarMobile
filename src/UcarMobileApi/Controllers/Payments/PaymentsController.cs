using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Application.DTOs;
using UcarMobileApi.Application.Services.Users;

namespace UcarMobileApi.Controllers.Payments;

/// <summary>
/// Handles HTTP endpoints related to payment setup, charges, and refunds.
/// </summary>
[ApiController]
[Route("api/payments")]
[AllowAnonymous]
public class PaymentsController(IPaymentService paymentService, CurrentUserService currentUser) : ControllerBase
{
    /// <summary>
    /// Initializes a payment setup process for a specific client.
    /// Allows registering a card without charging it.
    /// </summary>
    [HttpPost("setup-intent")]
    [ProducesResponseType(typeof(PaymentSetupDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaymentSetupDto>> CreateSetupIntent(CancellationToken cancellationToken)
    {
        var result = await paymentService.CreateSetupIntentAsync(currentUser.AuthProviderId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Attaches a payment method to a client and persists it locally.
    /// </summary>
    [HttpPost("attach-method")]
    [ProducesResponseType(typeof(PaymentMethodDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaymentMethodDto>> AttachPaymentMethod([FromBody] PaymentMethodAttachDto dto, CancellationToken cancellationToken)
    {
        var result = await paymentService.AttachPaymentMethodAsync(currentUser.AuthProviderId, dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Creates and confirms a payment for the given client.
    /// </summary>
    [HttpPost("charge")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaymentDto>> CreatePayment([FromBody] PaymentCreateDto dto, CancellationToken cancellationToken)
    {
        var result = await paymentService.CreatePaymentAsync(currentUser.AuthProviderId, dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Issues a refund for a specific payment.
    /// </summary>
    [HttpPost("refund")]
    [ProducesResponseType(typeof(PaymentRefundResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaymentRefundResultDto>> Refund([FromBody] PaymentRefundDto dto, CancellationToken cancellationToken)
    {
        var result = await paymentService.RefundPaymentAsync(dto, cancellationToken);
        return Ok(result);
    }
}
