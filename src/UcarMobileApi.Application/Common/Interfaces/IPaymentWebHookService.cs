using System.Threading;
using System.Threading.Tasks;
using Stripe;

namespace UcarMobileApi.Application.Common.Interfaces
{
    /// <summary>
    /// Defines contract for handling Stripe webhook events.
    /// </summary>
    public interface IPaymentWebHookService
    {
        /// <summary>
        /// Handles Stripe events related to <see cref="PaymentIntent"/> (e.g. succeeded, failed).
        /// </summary>
        /// <param name="intent">The Stripe payment intent received from the webhook.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        Task HandlePaymentIntentWebhookAsync(PaymentIntent intent, CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles Stripe events related to <see cref="Refund"/> (e.g. refunded, failed, updated).
        /// </summary>
        /// <param name="refund">The Stripe refund object received from the webhook.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        Task HandleRefundWebhookAsync(Refund refund, CancellationToken cancellationToken = default);

        // ====== Payouts ===== //

        /// <summary>
        /// Handles provider account.updated webhook events (e.g. enabling payouts/charges).
        /// </summary>
        Task HandleAccountUpdatedAsync(Account account);

        /// <summary>
        /// Handles provider payout webhooks (e.g. payout.paid, payout.failed).
        /// </summary>
        Task HandlePayoutWebhookAsync(Payout payout);
    }
}
