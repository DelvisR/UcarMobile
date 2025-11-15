using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using UcarMobileApi.Infrastructure.Factories;
using UcarMobileApi.Infrastructure.Services.Payments;

namespace UcarMobileApi.Controllers.Payments;

/// <summary>
/// Controller responsible for handling incoming Stripe webhook events.
/// This endpoint is called directly by Stripe when specific events occur
/// (e.g., payment succeeded, refund issued, etc.).
/// </summary>
[ApiController]
[Route("api/payments/webhook")]
public class StripeWebhookController(StripePaymentWebHookService paymentWebHookService, StripeClientFactory stripeClientFactory, ILogger<StripeWebhookController> logger) : ControllerBase
{
    /// <summary>
    /// Handles Stripe webhook requests.
    /// Validates the signature using the Webhook Secret and delegates the event to the appropriate service handler.
    /// </summary>
    /// <remarks>
    /// This endpoint must be publicly accessible by Stripe, but should not require authentication.
    /// Make sure to validate the signature using <see cref="StripeClientFactory.GetWebhookSecretAsync"/>.
    /// </remarks>
    /// <returns>HTTP 200 if processed successfully, or HTTP 400 if validation fails.</returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Handle()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var secret = await stripeClientFactory.GetWebhookSecretAsync();

        if (string.IsNullOrWhiteSpace(secret))
        {
            logger.LogError("Stripe Webhook secret is not configured.");
            return StatusCode(500, "Webhook secret not configured.");
        }

        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                secret
            );
        }
        catch (StripeException ex)
        {
            logger.LogWarning(ex, "Invalid Stripe webhook signature.");
            return BadRequest();
        }

        logger.LogInformation("Received Stripe event: {Type}", stripeEvent.Type);

        // Dispatch event type to proper handler
        switch (stripeEvent.Type)
        {
            case "payment_intent.succeeded":
            case "payment_intent.payment_failed":
                if (stripeEvent.Data.Object is PaymentIntent paymentIntent)
                    await paymentWebHookService.HandlePaymentIntentWebhookAsync(paymentIntent);
                break;

            case "charge.refunded":
            case "refund.updated":
            case "refund.failed":
                if (stripeEvent.Data.Object is Refund refund)
                    await paymentWebHookService.HandleRefundWebhookAsync(refund);
                break;

            default:
                logger.LogInformation("Unhandled Stripe event type: {Type}", stripeEvent.Type);
                break;
        }

        return Ok();
    }
}
