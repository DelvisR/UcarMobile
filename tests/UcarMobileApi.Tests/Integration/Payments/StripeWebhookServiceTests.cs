using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Stripe;
using UcarMobileApi.Core.Entities.Payments;
using UcarMobileApi.Infrastructure.Data;
using UcarMobileApi.Infrastructure.Services.Payments;
using Xunit;
using Xunit.Abstractions;

namespace UcarMobileApi.Tests.Integration.Payments
{
    /// <summary>
    /// Integration tests for StripePaymentWebHookService
    /// Tests webhook event processing scenarios
    /// </summary>
    public class StripeWebhookServiceTests : IClassFixture<StripeTestFixture>, IDisposable
    {
        private readonly AppDbContext _context;
        private readonly StripePaymentWebHookService _webhookService;
        private readonly ITestOutputHelper _output;
        private readonly Mock<ILogger<StripePaymentWebHookService>> _loggerMock;

        public StripeWebhookServiceTests(StripeTestFixture fixture, ITestOutputHelper output)
        {
            _output = output;
            _context = fixture.CreateDbContext();
            _loggerMock = new Mock<ILogger<StripePaymentWebHookService>>();
            _webhookService = new StripePaymentWebHookService(_context, _loggerMock.Object);
        }

        #region PaymentIntent Webhook Tests

        [Fact]
        public async Task HandlePaymentIntentWebhookAsync_NewPaymentIntent_CreatesPaymentRecord()
        {
            // Arrange
            var paymentIntent = CreateTestPaymentIntent("pi_test_new_payment", "succeeded", 2500);

            // Act
            await _webhookService.HandlePaymentIntentWebhookAsync(paymentIntent);

            // Assert
            var payment = await _context.Set<Payment>()
                .FirstOrDefaultAsync(p => p.ProviderPaymentId == paymentIntent.Id);

            payment.Should().NotBeNull();
            payment.ProviderPaymentId.Should().Be(paymentIntent.Id);
            payment.Status.Should().Be("succeeded");
            payment.AmountCents.Should().Be(2500);
            payment.Currency.Should().Be("usd");

            _output.WriteLine($"Payment created from webhook: {payment.ProviderPaymentId}");
        }

        [Fact]
        public async Task HandlePaymentIntentWebhookAsync_ExistingPaymentIntent_UpdatesStatus()
        {
            // Arrange
            const string paymentId = "pi_test_existing_payment";

            // Create existing payment in database
            var existingPayment = new Payment
            {
                ProviderPaymentId = paymentId,
                AmountCents = 3000,
                Currency = "usd",
                Status = "processing"
            };
            _context.Set<Payment>().Add(existingPayment);
            await _context.SaveChangesAsync();

            var paymentIntent = CreateTestPaymentIntent(paymentId, "succeeded", 3000);

            // Act
            await _webhookService.HandlePaymentIntentWebhookAsync(paymentIntent);

            // Assert
            var updatedPayment = await _context.Set<Payment>()
                .FirstOrDefaultAsync(p => p.ProviderPaymentId == paymentId);

            updatedPayment.Should().NotBeNull();
            updatedPayment.Status.Should().Be("succeeded");

            _output.WriteLine($"Payment status updated: {paymentId} -> succeeded");
        }

        [Theory]
        [InlineData("succeeded")]
        [InlineData("requires_payment_method")]
        [InlineData("requires_confirmation")]
        [InlineData("requires_action")]
        [InlineData("processing")]
        [InlineData("canceled")]
        public async Task HandlePaymentIntentWebhookAsync_DifferentStatuses_HandlesCorrectly(string status)
        {
            // Arrange
            var paymentId = $"pi_test_status_{status}_{Guid.NewGuid()}";
            var paymentIntent = CreateTestPaymentIntent(paymentId, status, 1500);

            // Act
            await _webhookService.HandlePaymentIntentWebhookAsync(paymentIntent);

            // Assert
            var payment = await _context.Set<Payment>()
                .FirstOrDefaultAsync(p => p.ProviderPaymentId == paymentId);

            payment.Should().NotBeNull();
            payment.Status.Should().Be(status);

            _output.WriteLine($"Payment with status '{status}' processed correctly");
        }

        #endregion

        #region Refund Webhook Tests

        [Fact]
        public async Task HandleRefundWebhookAsync_NewRefund_CreatesRefundRecord()
        {
            // Arrange
            const string paymentId = "pi_test_refund_payment";

            // Create parent payment
            var payment = new Payment
            {
                ProviderPaymentId = paymentId,
                AmountCents = 5000,
                Currency = "usd",
                Status = "succeeded"
            };
            _context.Set<Payment>().Add(payment);
            await _context.SaveChangesAsync();

            var refund = CreateTestRefund("re_test_new_refund", paymentId, 2000, "succeeded");

            // Act
            await _webhookService.HandleRefundWebhookAsync(refund);

            // Assert
            var refundRecord = await _context.Set<PaymentRefund>()
                .FirstOrDefaultAsync(r => r.ProviderRefundId == refund.Id);

            refundRecord.Should().NotBeNull();
            refundRecord.ProviderRefundId.Should().Be(refund.Id);
            refundRecord.PaymentId.Should().Be(payment.Id);
            refundRecord.AmountCents.Should().Be(2000);
            refundRecord.Status.Should().Be("succeeded");

            // Check payment status is updated
            var updatedPayment = await _context.Set<Payment>().FindAsync(payment.Id);
            if (updatedPayment != null)
            {
                updatedPayment.Status.Should().Be("partial_refunded");

                _output.WriteLine($"Refund created: {refund.Id}, Payment status: {updatedPayment.Status}");
            }
        }

        [Fact]
        public async Task HandleRefundWebhookAsync_FullRefund_UpdatesPaymentToRefunded()
        {
            // Arrange
            const string paymentId = "pi_test_full_refund";

            // Create parent payment
            var payment = new Payment
            {
                ProviderPaymentId = paymentId,
                AmountCents = 3000,
                Currency = "usd",
                Status = "succeeded"
            };
            _context.Set<Payment>().Add(payment);
            await _context.SaveChangesAsync();

            var refund = CreateTestRefund("re_test_full_refund", paymentId, 3000, "succeeded");

            // Act
            await _webhookService.HandleRefundWebhookAsync(refund);

            // Assert
            var updatedPayment = await _context.Set<Payment>().FindAsync(payment.Id);
            if (updatedPayment != null)
            {
                updatedPayment.Status.Should().Be("refunded");

                _output.WriteLine($"Full refund processed, payment status: {updatedPayment.Status}");
            }
        }

        [Fact]
        public async Task HandleRefundWebhookAsync_MultiplePartialRefunds_CalculatesCorrectStatus()
        {
            // Arrange
            const string paymentId = "pi_test_multiple_refunds";

            // Create parent payment
            var payment = new Payment
            {
                ProviderPaymentId = paymentId,
                AmountCents = 10000,
                Currency = "usd",
                Status = "succeeded"
            };
            _context.Set<Payment>().Add(payment);
            await _context.SaveChangesAsync();

            // First partial refund
            var firstRefund = CreateTestRefund("re_test_first_partial", paymentId, 3000, "succeeded");
            await _webhookService.HandleRefundWebhookAsync(firstRefund);

            // Second partial refund
            var secondRefund = CreateTestRefund("re_test_second_partial", paymentId, 4000, "succeeded");
            await _webhookService.HandleRefundWebhookAsync(secondRefund);

            // Third refund that completes the full amount
            var thirdRefund = CreateTestRefund("re_test_third_partial", paymentId, 3000, "succeeded");

            // Act
            await _webhookService.HandleRefundWebhookAsync(thirdRefund);

            // Assert
            var updatedPayment = await _context.Set<Payment>().FindAsync(payment.Id);
            if (updatedPayment != null)
            {
                updatedPayment.Status.Should().Be("refunded");

                var totalRefunds = await _context.Set<PaymentRefund>()
                    .Where(r => r.PaymentId == payment.Id && r.Status == "succeeded")
                    .SumAsync(r => r.AmountCents);

                totalRefunds.Should().Be(10000);

                _output.WriteLine(
                    $"Multiple refunds totaling ${totalRefunds / 100.0:F2}, final status: {updatedPayment.Status}");
            }
        }

        [Fact]
        public async Task HandleRefundWebhookAsync_FailedRefund_LogsWarning()
        {
            // Arrange
            const string paymentId = "pi_test_failed_refund";

            // Create parent payment
            var payment = new Payment
            {
                ProviderPaymentId = paymentId,
                AmountCents = 2000,
                Currency = "usd",
                Status = "succeeded"
            };
            _context.Set<Payment>().Add(payment);
            await _context.SaveChangesAsync();

            var refund = CreateTestRefund("re_test_failed", paymentId, 2000, "failed");
            refund.FailureReason = "insufficient_funds";

            // Act
            await _webhookService.HandleRefundWebhookAsync(refund);

            // Assert
            var refundRecord = await _context.Set<PaymentRefund>()
                .FirstOrDefaultAsync(r => r.ProviderRefundId == refund.Id);

            refundRecord.Should().NotBeNull();
            refundRecord.Status.Should().Be("failed");

            // Payment status should not change for failed refund
            var updatedPayment = await _context.Set<Payment>().FindAsync(payment.Id);
            updatedPayment?.Status.Should().Be("succeeded");

            // Verify warning was logged
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("failed")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

            _output.WriteLine($"Failed refund logged correctly: {refund.FailureReason}");
        }

        [Fact]
        public async Task HandleRefundWebhookAsync_UnknownPayment_LogsWarning()
        {
            // Arrange
            const string unknownPaymentId = "pi_unknown_payment";
            var refund = CreateTestRefund("re_test_unknown", unknownPaymentId, 1000, "succeeded");

            // Act
            await _webhookService.HandleRefundWebhookAsync(refund);

            // Assert
            // Verify warning was logged for unknown payment
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("unknown PaymentIntent")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

            _output.WriteLine("Unknown payment refund handled correctly");
        }

        [Fact]
        public async Task HandleRefundWebhookAsync_ExistingRefund_UpdatesStatus()
        {
            // Arrange
            var paymentId = "pi_test_existing_refund";

            // Create parent payment
            var payment = new Payment
            {
                ProviderPaymentId = paymentId,
                AmountCents = 4000,
                Currency = "usd",
                Status = "succeeded"
            };
            _context.Set<Payment>().Add(payment);
            await _context.SaveChangesAsync();

            // Create existing refund in pending status
            var existingRefund = new PaymentRefund
            {
                PaymentId = payment.Id,
                ProviderRefundId = "re_test_existing",
                AmountCents = 4000,
                Currency = "usd",
                Status = "pending"
            };
            _context.Set<PaymentRefund>().Add(existingRefund);
            await _context.SaveChangesAsync();

            var refund = CreateTestRefund("re_test_existing", paymentId, 4000, "succeeded");

            // Act
            await _webhookService.HandleRefundWebhookAsync(refund);

            // Assert
            var updatedRefund = await _context.Set<PaymentRefund>()
                .FirstOrDefaultAsync(r => r.ProviderRefundId == refund.Id);

            updatedRefund.Should().NotBeNull();
            updatedRefund.Status.Should().Be("succeeded");

            _output.WriteLine("Existing refund status updated: pending -> succeeded");
        }

        #endregion

        #region Helper Methods

        private static PaymentIntent CreateTestPaymentIntent(string id, string status, long amount)
        {
            return new PaymentIntent
            {
                Id = id,
                Status = status,
                Amount = amount,
                Currency = "usd",
                Created = DateTime.UtcNow,
                Metadata = new Dictionary<string, string>
                {
                    { "source", "UcarMobile" },
                    { "test", "true" }
                }
            };
        }

        private static Refund CreateTestRefund(string id, string paymentIntentId, long amount, string status)
        {
            return new Refund
            {
                Id = id,
                PaymentIntent = new PaymentIntent { Id = paymentIntentId },
                Amount = amount,
                Currency = "usd",
                Status = status,
                Created = DateTime.UtcNow,
                Metadata = new Dictionary<string, string>
                {
                    { "source", "UcarMobile" },
                    { "test", "true" }
                }
            };
        }

        #endregion

        public void Dispose()
        {
            _context?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
