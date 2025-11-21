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
public class PaymentsController(IPaymentService paymentService, CurrentUserService currentUser) : ControllerBase
{
    /// <summary>
    /// Returns all saved payment methods for the current authenticated user.
    /// </summary>
    [HttpGet("methods")]
    [ProducesResponseType(typeof(IEnumerable<PaymentMethodListDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PaymentMethodListDto>>> GetPaymentMethods(CancellationToken cancellationToken)
    {
        var result = await paymentService.GetPaymentMethodsAsync(currentUser.AuthProviderId, cancellationToken);
        return Ok(result);
    }

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
    /// Sets a payment method as the default for the current user.
    /// </summary>
    [HttpPost("methods/{id:int}/set-default")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SetDefaultPaymentMethod(int id, CancellationToken cancellationToken)
    {
        await paymentService.SetDefaultPaymentMethodAsync(currentUser.AuthProviderId, id, cancellationToken);
        return NoContent();
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
