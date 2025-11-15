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
    }
}
