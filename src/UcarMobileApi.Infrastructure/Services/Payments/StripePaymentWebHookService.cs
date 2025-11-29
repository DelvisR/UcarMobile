using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stripe;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Core.Entities.Payments;
using UcarMobileApi.Core.Entities.Technicians;
using UcarMobileApi.Infrastructure.Data;
using Payout = UcarMobileApi.Core.Entities.Payments.Payout;

namespace UcarMobileApi.Infrastructure.Services.Payments;

/// <summary>
/// Handles processing of Stripe webhook events, updating the database accordingly.
/// </summary>
public class StripePaymentWebHookService(AppDbContext context, ILogger<StripePaymentWebHookService> logger) : IPaymentWebHookService
{
    /// <summary>
    /// Processes Stripe PaymentIntent webhook events (e.g. succeeded or failed).
    /// Updates or creates the corresponding <see cref="Payment"/> record in the database.
    /// </summary>
    public async Task HandlePaymentIntentWebhookAsync(PaymentIntent intent, CancellationToken cancellationToken = default)
    {
        var existing = await context.Set<Payment>()
            .FirstOrDefaultAsync(p => p.ProviderPaymentId == intent.Id, cancellationToken);

        if (existing == null)
        {
            existing = new Payment
            {
                ProviderPaymentId = intent.Id,
                AmountCents = intent.Amount,
                Currency = intent.Currency,
                Status = intent.Status
            };
            await context.Set<Payment>().AddAsync(existing, cancellationToken);
        }
        else
        {
            existing.Status = intent.Status;
        }

        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Updated payment {PaymentId} -> {Status}", intent.Id, intent.Status);

        //todo if failed send notification to admins (not implemented here)
    }

    /// <summary>
    /// Processes Stripe Refund webhook events (e.g. succeeded, failed, updated).
    /// Updates or inserts <see cref="PaymentRefund"/> and adjusts the parent payment status accordingly.
    /// </summary>
    public async Task HandleRefundWebhookAsync(Refund refund, CancellationToken cancellationToken = default)
    {
        // Find the parent payment associated with the refund
        var payment = await context.Set<Payment>()
            .FirstOrDefaultAsync(p => p.ProviderPaymentId == refund.PaymentIntent.Id, cancellationToken);

        if (payment == null)
        {
            logger.LogWarning("Refund received for unknown PaymentIntent {Id}", refund.PaymentIntent);
            return;
        }

        var existingRefund = await context.Set<PaymentRefund>()
            .FirstOrDefaultAsync(r => r.ProviderRefundId == refund.Id, cancellationToken);

        if (existingRefund == null)
        {
            existingRefund = new PaymentRefund
            {
                PaymentId = payment.Id,
                ProviderRefundId = refund.Id,
                AmountCents = refund.Amount,
                Currency = refund.Currency ?? payment.Currency,
                Status = refund.Status ?? "unknown"
            };
            await context.Set<PaymentRefund>().AddAsync(existingRefund, cancellationToken);
        }
        else
        {
            existingRefund.Status = refund.Status ?? existingRefund.Status;
        }

        // Update the payment based on total refunded amount
        switch (refund.Status)
        {
            case "succeeded":
                var totalRefunded = await context.Set<PaymentRefund>()
                    .Where(r => r.PaymentId == payment.Id && r.Status == "succeeded")
                    .SumAsync(r => r.AmountCents, cancellationToken);

                totalRefunded += existingRefund.AmountCents;

                if (totalRefunded >= payment.AmountCents)
                    payment.Status = "refunded";
                else if (totalRefunded > 0)
                    payment.Status = "partial_refunded";
                break;

            case "failed":
                logger.LogWarning("Refund {RefundId} failed for payment {PaymentIntent}. Reason: {Reason}",
                    refund.Id, refund.PaymentIntent, refund.FailureReason ?? "unknown");
                break;
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    // ===== PAYOUTS ===== //

    public async Task HandleAccountUpdatedAsync(Account account)
    {
        var tech = await context.Set<Technician>().FirstOrDefaultAsync(t => t.ProviderAccountId == account.Id);

        if (tech == null)
            return;

        tech.ProviderPaymentsEnabled = account is { ChargesEnabled: true, PayoutsEnabled: true };

        tech.ProviderDisplayName = $"{account.Individual?.FirstName} {account.Individual?.LastName}".Trim();

        await context.SaveChangesAsync();

        logger.LogInformation("Updated technician {TechnicianId} provider flags: PaymentsEnabled={Enabled}", tech.Id, tech.ProviderPaymentsEnabled);
    }

    /// <summary>
    /// Updates an existing payout record when the provider sends a payout webhook event.
    /// </summary>
    public async Task HandlePayoutWebhookAsync(Stripe.Payout payout)
    {
        var payoutRecord = await context.Set<Payout>().FirstOrDefaultAsync(p => p.ProviderPayoutId == payout.Id);

        if (payoutRecord == null)
            return;

        payoutRecord.ProviderDestination = payout.Destination.AccountId;
        payoutRecord.Status = payout.Status ?? "unknown";
        payoutRecord.CreatedAt = payout.Created;

        await context.SaveChangesAsync();

        logger.LogInformation("Recorded provider payout {PayoutId} status {Status} amount {Amount}", payout.Id, payout.Status, payout.Amount);
    }
}
