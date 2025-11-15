using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Application.DTOs;

namespace UcarMobileApi.Controllers.Payments;

/// <summary>
/// Handles HTTP endpoints related to payment setup, charges, and refunds.
/// </summary>
[ApiController]
[Route("api/payments")]
[AllowAnonymous]
public class PaymentsController(IPaymentService paymentService) : ControllerBase
{
    /// <summary>
    /// Initializes a payment setup process for a specific client.
    /// Allows registering a card without charging it.
    /// </summary>
    [HttpPost("setup-intent/{clientId:int}")]
    [ProducesResponseType(typeof(PaymentSetupDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaymentSetupDto>> CreateSetupIntent(int clientId, CancellationToken cancellationToken)
    {
        var result = await paymentService.CreateSetupIntentAsync(clientId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Saves a confirmed payment method in the database after confirmSetup.
    /// </summary>
    [HttpPost("save-method")]
    [ProducesResponseType(typeof(PaymentMethodDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaymentMethodDto>> SavePaymentMethodAsync([FromBody] PaymentMethodCreateDto dto, CancellationToken cancellationToken)
    {
        var result = await paymentService.SavePaymentMethodAsync(dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Attaches a payment method to a client and persists it locally.
    /// </summary>
    [HttpPost("attach-method")]
    [ProducesResponseType(typeof(PaymentMethodDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaymentMethodDto>> AttachPaymentMethod([FromBody] PaymentMethodAttachDto dto, CancellationToken cancellationToken)
    {
        var result = await paymentService.AttachPaymentMethodAsync(dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Creates and confirms a payment for the given client.
    /// </summary>
    [HttpPost("charge")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaymentDto>> CreatePayment([FromBody] PaymentCreateDto dto, CancellationToken cancellationToken)
    {
        var result = await paymentService.CreatePaymentAsync(dto, cancellationToken);
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
