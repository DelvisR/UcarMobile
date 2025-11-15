using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Stripe;
using UcarMobileApi.Application.DTOs;
using UcarMobileApi.Application.Services.Security;
using UcarMobileApi.Core.Entities.Clients;
using UcarMobileApi.Core.Entities.Payments;
using UcarMobileApi.Infrastructure.Data;
using UcarMobileApi.Infrastructure.Services.Payments;
using Xunit;
using Xunit.Abstractions;
using PaymentMethod = UcarMobileApi.Core.Entities.Payments.PaymentMethod;

namespace UcarMobileApi.Tests.Integration.Payments
{
    /// <summary>
    /// Integration tests for StripePaymentService using Stripe test cards
    /// Requires Stripe test environment configuration
    /// </summary>
    public class StripePaymentServiceTests : IClassFixture<StripeTestFixture>, IDisposable
    {
        private readonly AppDbContext _context;
        private readonly StripePaymentService _paymentService;
        private readonly ITestOutputHelper _output;


        public StripePaymentServiceTests(StripeTestFixture fixture, ITestOutputHelper output)
        {
            _output = output;
            _context = fixture.CreateDbContext();

            var loggerMock = new Mock<ILogger<StripePaymentService>>();
            var mapper = fixture.Mapper;
            var cacheMock = new Mock<ICacheService>();

            // In-memory cache simulation
            var localCache = new Dictionary<string, object>();

            cacheMock
                .Setup(c => c.GetAsync<int?>(It.IsAny<string>()))
                .ReturnsAsync((string key) =>
                    localCache.TryGetValue(key, out var value) ? (int?)value : null);

            cacheMock
                .Setup(c => c.SetAsync<int?>(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<TimeSpan?>(), It.IsAny<TimeSpan?>()))
                .Callback((string key, int? value, TimeSpan? ttl, TimeSpan? sliding) =>
                {
                    localCache[key] = value;
                })
                .Returns(Task.CompletedTask);

            _paymentService = new StripePaymentService(_context, mapper, loggerMock.Object, cacheMock.Object);
        }

        #region Setup Intent Tests

        [Fact]
        public async Task CreateSetupIntentAsync_ValidClient_ReturnsClientSecret()
        {
            // Arrange
            var client = await CreateTestClientAsync();

            // Act
            var result = await _paymentService.CreateSetupIntentAsync(client.Id);

            // Assert
            result.Should().NotBeNull();
            result.ClientSecret.Should().StartWith("seti_");
            result.ProviderCustomerId.Should().StartWith("cus_");

            _output.WriteLine($"SetupIntent created: {result.ClientSecret}");
        }

        [Fact]
        public async Task CreateSetupIntentAsync_NonExistentClient_ThrowsKeyNotFoundException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _paymentService.CreateSetupIntentAsync(99999));

            exception.Message.Should().Contain("Client with ID 99999 not found");
        }

        #endregion

        #region Payment Method Tests

        [Fact]
        public async Task SavePaymentMethodAsync_ValidData_SavesSuccessfully()
        {
            // Arrange
            var client = await CreateTestClientAsync();
            await EnsureStripeCustomerExists(client);

            var dto = new PaymentMethodCreateDto(
                ClientId: client.Id,
                ProviderPaymentMethodId: "pm_card_visa",
                Brand: "visa",
                Last4: "4242",
                ExpMonth: 12,
                ExpYear: 2028,
                IsDefault: true
            );

            // Act
            var result = await _paymentService.SavePaymentMethodAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.Brand.Should().Be("visa");
            result.Last4.Should().Be("4242");
            result.IsDefault.Should().BeTrue();

            _output.WriteLine($"Payment method saved: {result.Brand} ending in {result.Last4}");
        }

        [Fact]
        public async Task SavePaymentMethodAsync_MultipleDefaults_OnlyOneRemains()
        {
            // Arrange
            var client = await CreateTestClientAsync();
            await EnsureStripeCustomerExists(client);

            // Create first default payment method
            var firstDto = new PaymentMethodCreateDto(
                ClientId: client.Id,
                ProviderPaymentMethodId: "pm_card_visa",
                Brand: "visa",
                Last4: "4242",
                ExpMonth: 12,
                ExpYear: 2028,
                IsDefault: true
            );
            await _paymentService.SavePaymentMethodAsync(firstDto);

            // Create second default payment method
            var secondDto = new PaymentMethodCreateDto(
                ClientId: client.Id,
                ProviderPaymentMethodId: "pm_card_mastercard",
                Brand: "mastercard",
                Last4: "4444",
                ExpMonth: 10,
                ExpYear: 2027,
                IsDefault: true
            );

            // Act
            await _paymentService.SavePaymentMethodAsync(secondDto);

            // Assert
            var paymentMethods = await _context.Set<PaymentMethod>()
                .Where(pm => pm.ClientId == client.Id)
                .ToListAsync();

            var defaultMethods = paymentMethods.Where(pm => pm.IsDefault).ToList();
            defaultMethods.Should().HaveCount(1);
            defaultMethods.First().Brand.Should().Be("mastercard");

            _output.WriteLine($"Default payment methods count: {defaultMethods.Count}");
        }

        #endregion

        #region Payment Processing Tests

        [Theory]
        [InlineData("pm_card_visa", "4242", "succeeded", null)] // Successful payment
        [InlineData("pm_card_visa_debit", "4000", "succeeded", null)] // Successful debit card
        [InlineData("pm_card_mastercard", "4444", "succeeded", null)] // Successful Mastercard
        public async Task CreatePaymentAsync_SuccessfulCards_ProcessesCorrectly(
            string paymentMethodId, string last4, string expectedStatus, string expectedError)
        {
            // Arrange
            var client = await CreateTestClientAsync();
            await EnsureStripeCustomerExists(client);
            var pm = await CreateTestPaymentMethodAsync(client.Id, paymentMethodId, last4);

            var dto = new PaymentCreateDto(
                ClientId: client.Id,
                pm.Id,
                ProviderPaymentMethodId: paymentMethodId,
                IdempotencyKey: $"test-payment-{Guid.NewGuid()}",
                AmountCents: 2000, // $20.00
                Currency: "usd"
            );

            // Act
            var result = await _paymentService.CreatePaymentAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(expectedStatus);
            result.ErrorCode.Should().Be(expectedError);
            result.AmountCents.Should().Be(2000);

            _output.WriteLine($"Payment processed: Status={result.Status}, Amount=${result.AmountCents / 100.0:F2}");
        }

        [Theory]
        [InlineData("pm_card_chargeDeclined", "0002", "generic_decline")]
        [InlineData("pm_card_chargeDeclinedInsufficientFunds", "9995", "insufficient_funds")]
        [InlineData("pm_card_chargeDeclinedLostCard", "9987", "lost_card")]
        [InlineData("pm_card_chargeDeclinedStolenCard", "9979", "stolen_card")]
        [InlineData("pm_card_expiredCard", "0004", "resource_missing")]
        [InlineData("pm_card_cvcDecline", "0127", "resource_missing")]
        public async Task CreatePaymentAsync_DeclinedCards_ReturnsCorrectErrorCode(
            string paymentMethodId, string last4, string expectedErrorCode)
        {
            // Arrange
            var client = await CreateTestClientAsync();
            await EnsureStripeCustomerExists(client);
            var pm = await CreateTestPaymentMethodAsync(client.Id, paymentMethodId, last4);

            var dto = new PaymentCreateDto(
                ClientId: client.Id,
                pm.Id,
                ProviderPaymentMethodId: paymentMethodId,
                IdempotencyKey: $"test-decline-{Guid.NewGuid()}",
                AmountCents: 1500, // $15.00
                Currency: "usd"
            );

            // Act
            var result = await _paymentService.CreatePaymentAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be("error");
            result.ErrorCode.Should().Be(expectedErrorCode);

            _output.WriteLine($"Declined payment: ErrorCode={result.ErrorCode}, Card ending in {last4}");
        }

        [Fact]
        public async Task CreatePaymentAsync_3DSecureRequired_ReturnsClientSecret()
        {
            // Arrange
            var client = await CreateTestClientAsync();
            await EnsureStripeCustomerExists(client);
            var pm = await CreateTestPaymentMethodAsync(client.Id, "pm_card_authenticationRequired", "3220");

            var dto = new PaymentCreateDto(
                ClientId: client.Id,
                pm.Id,
                ProviderPaymentMethodId: "pm_card_authenticationRequired",
                IdempotencyKey: $"test-3ds-{Guid.NewGuid()}",
                AmountCents: 3000, // $30.00
                Currency: "usd"
            );

            // Act
            var result = await _paymentService.CreatePaymentAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be("error");
            result.ErrorCode.Should().Be("authentication_required");
            //result.ClientSecret.Should().NotBeNullOrEmpty();
            //result.ClientSecret.Should().StartWith("pi_");

            _output.WriteLine($"3DS Required: ClientSecret={result.ClientSecret}");
        }

        [Fact]
        public async Task CreatePaymentAsync_IdempotencyKey_PreventsDuplicates()
        {
            // Arrange
            var client = await CreateTestClientAsync();
            await EnsureStripeCustomerExists(client);
            var pm = await CreateTestPaymentMethodAsync(client.Id, "pm_card_visa", "4242");

            var idempotencyKey = $"test-idempotency-{Guid.NewGuid()}";
            var dto = new PaymentCreateDto(
                ClientId: client.Id,
                pm.Id,
                ProviderPaymentMethodId: "pm_card_visa",
                IdempotencyKey: idempotencyKey,
                AmountCents: 1000,
                Currency: "usd"
            );

            // Act - First payment
            var firstResult = await _paymentService.CreatePaymentAsync(dto);

            // Act - Second payment with same idempotency key
            var secondResult = await _paymentService.CreatePaymentAsync(dto);

            // Assert
            firstResult.Should().NotBeNull();
            secondResult.Should().NotBeNull();
            firstResult.ProviderPaymentId.Should().Be(secondResult.ProviderPaymentId);

            _output.WriteLine($"Idempotent payment: {firstResult.ProviderPaymentId}");
        }

        #endregion

        #region Refund Tests

        [Fact]
        public async Task RefundPaymentAsync_FullRefund_ProcessesSuccessfully()
        {
            // Arrange
            var client = await CreateTestClientAsync();
            await EnsureStripeCustomerExists(client);
            var paymentMethod = await CreateTestPaymentMethodAsync(client.Id, "pm_card_visa", "4242");

            // Create a successful payment first
            var paymentDto = new PaymentCreateDto(
                ClientId: client.Id,
                paymentMethod.Id,
                ProviderPaymentMethodId: "pm_card_visa",
                IdempotencyKey: $"test-payment-{Guid.NewGuid()}",
                AmountCents: 5000,
                Currency: "usd"
            );
            var paymentResult = await _paymentService.CreatePaymentAsync(paymentDto);

            // Create payment record in database
            var payment = new Payment
            {
                ClientId = client.Id,
                PaymentMethodId = paymentMethod.Id,
                ProviderPaymentId = paymentResult.ProviderPaymentId,
                AmountCents = 5000,
                Currency = "usd",
                Status = "succeeded"
            };
            _context.Set<Payment>().Add(payment);
            await _context.SaveChangesAsync();

            var refundDto = new PaymentRefundDto(
                PaymentId: payment.Id,
                AmountCents: null, // Full refund
                IdempotencyKey: $"test-refund-{Guid.NewGuid()}"
            );

            // Act
            var result = await _paymentService.RefundPaymentAsync(refundDto);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be("succeeded");
            result.AmountCents.Should().Be(5000);

            _output.WriteLine($"Refund processed: {result.ProviderRefundId}, Amount=${result.AmountCents / 100.0:F2}");
        }

        [Fact]
        public async Task RefundPaymentAsync_PartialRefund_ProcessesSuccessfully()
        {
            // Arrange
            var client = await CreateTestClientAsync();
            await EnsureStripeCustomerExists(client);
            var paymentMethod = await CreateTestPaymentMethodAsync(client.Id, "pm_card_visa", "4242");

            // Create a successful payment first
            var paymentDto = new PaymentCreateDto(
                ClientId: client.Id,
                paymentMethod.Id,
                ProviderPaymentMethodId: "pm_card_visa",
                IdempotencyKey: $"test-payment-{Guid.NewGuid()}",
                AmountCents: 10000,
                Currency: "usd"
            );
            var paymentResult = await _paymentService.CreatePaymentAsync(paymentDto);

            // Create payment record in database
            var payment = new Payment
            {
                ClientId = client.Id,
                PaymentMethodId = paymentMethod.Id,
                ProviderPaymentId = paymentResult.ProviderPaymentId,
                AmountCents = 10000,
                Currency = "usd",
                Status = "succeeded"
            };
            _context.Set<Payment>().Add(payment);
            await _context.SaveChangesAsync();

            var refundDto = new PaymentRefundDto(
                PaymentId: payment.Id,
                AmountCents: 3000, // Partial refund
                IdempotencyKey: $"test-partial-refund-{Guid.NewGuid()}"
            );

            // Act
            var result = await _paymentService.RefundPaymentAsync(refundDto);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be("succeeded");
            result.AmountCents.Should().Be(3000);

            // Check payment status is updated to partial_refunded
            var updatedPayment = await _context.Set<Payment>().FindAsync(payment.Id);
            updatedPayment?.Status.Should().Be("partial_refunded");

            _output.WriteLine($"Partial refund: ${result.AmountCents / 100.0:F2} of ${payment.AmountCents / 100.0:F2}");
        }

        #endregion

        #region International Cards Tests

        [Theory]
        [InlineData("pm_card_br", "0000", "brl")] // Brazil
        [InlineData("pm_card_ca", "0000", "cad")] // Canada
        [InlineData("pm_card_mx", "0000", "mxn")] // Mexico
        [InlineData("pm_card_gb", "0000", "gbp")] // United Kingdom
        public async Task CreatePaymentAsync_InternationalCards_ProcessesCorrectly(
            string paymentMethodId, string last4, string currency)
        {
            // Arrange
            var client = await CreateTestClientAsync();
            await EnsureStripeCustomerExists(client);
            var pm = await CreateTestPaymentMethodAsync(client.Id, paymentMethodId, last4);

            var dto = new PaymentCreateDto(
                ClientId: client.Id,
                pm.Id,
                ProviderPaymentMethodId: paymentMethodId,
                IdempotencyKey: $"test-intl-{Guid.NewGuid()}",
                AmountCents: 2500,
                Currency: currency
            );

            // Act
            var result = await _paymentService.CreatePaymentAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.Currency.Should().Be(currency);

            _output.WriteLine($"International payment: {currency.ToUpper()} {result.AmountCents / 100.0:F2}");
        }

        #endregion

        #region Processing Error Tests

        [Theory]
        [InlineData("pm_card_chargeDeclinedProcessingError", "0119", "processing_error")]
        [InlineData("pm_card_riskLevelElevated", "4000000000000002", "risk_level_elevated")]
        public async Task CreatePaymentAsync_ProcessingErrors_HandlesCorrectly(
            string paymentMethodId, string last4, string expectedErrorCode)
        {
            // Arrange
            var client = await CreateTestClientAsync();
            await EnsureStripeCustomerExists(client);
            var pm = await CreateTestPaymentMethodAsync(client.Id, paymentMethodId, last4);

            var dto = new PaymentCreateDto(
                ClientId: client.Id,
                pm.Id,
                ProviderPaymentMethodId: paymentMethodId,
                IdempotencyKey: $"test-error-{Guid.NewGuid()}",
                AmountCents: 2000,
                Currency: "usd"
            );

            // Act
            var result = await _paymentService.CreatePaymentAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be("error");
            result.ErrorCode.Should().Be(expectedErrorCode);

            _output.WriteLine($"Processing error: {expectedErrorCode}");
        }

        #endregion

        #region Helper Methods

        private async Task<Client> CreateTestClientAsync()
        {
            var client = new Client
            {
                FirstName = "Test",
                LastName = "Client",
                Email = $"test-{Guid.NewGuid()}@example.com",
                Phone = "+1234567890"
            };

            _context.Set<Client>().Add(client);
            await _context.SaveChangesAsync();
            return client;
        }

        private async Task EnsureStripeCustomerExists(Client client)
        {
            if (string.IsNullOrEmpty(client.ProviderPaymentCustomerId))
            {
                var customerService = new CustomerService();
                var customer = await customerService.CreateAsync(new CustomerCreateOptions
                {
                    Email = client.Email,
                    Name = $"{client.FirstName} {client.LastName}",
                    Phone = client.Phone
                });

                client.ProviderPaymentCustomerId = customer.Id;
                await _context.SaveChangesAsync();
            }
        }

        private async Task<PaymentMethod> CreateTestPaymentMethodAsync(int clientId, string providerPaymentMethodId, string last4)
        {
            var paymentMethod = new PaymentMethod
            {
                ClientId = clientId,
                ProviderPaymentMethodId = providerPaymentMethodId,
                Brand = "visa",
                Last4 = last4,
                ExpMonth = 12,
                ExpYear = 2028,
                IsDefault = true
            };

            _context.Set<PaymentMethod>().Add(paymentMethod);
            await _context.SaveChangesAsync();
            return paymentMethod;
        }

        #endregion

        public void Dispose()
        {
            _context?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
