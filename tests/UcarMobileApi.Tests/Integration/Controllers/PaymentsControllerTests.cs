using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using UcarMobileApi.Application.DTOs;
using UcarMobileApi.Tests.Unit.Payments;
using Xunit;
using Xunit.Abstractions;

namespace UcarMobileApi.Tests.Integration.Controllers
{
    /// <summary>
    /// Integration tests for PaymentsController
    /// Tests HTTP endpoints with various Stripe test scenarios
    /// </summary>
    public class PaymentsControllerTests(WebApplicationFactory<Program> factory, ITestOutputHelper output) : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly HttpClient _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Add test-specific services here if needed
                    // For example, override Stripe configuration for testing
                });
            }).CreateClient();
        private readonly ITestOutputHelper _output = output;

        public static class JsonOptions
        {
            public static readonly JsonSerializerOptions CamelCase = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        #region Setup Intent Tests

        [Fact]
        public async Task CreateSetupIntent_ValidClientId_ReturnsSuccess()
        {
            // Arrange
            var clientId = await CreateTestClientAsync();

            // Act
            var response = await _client.PostAsync($"/api/payments/setup-intent/{clientId}", null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PaymentSetupDto>(content, JsonOptions.CamelCase);

            result.Should().NotBeNull();
            result.ClientSecret.Should().StartWith("seti_");
            result.ProviderCustomerId.Should().StartWith("cus_");

            _output.WriteLine($"SetupIntent created via API: {result.ClientSecret}");
        }

        [Fact]
        public async Task CreateSetupIntent_InvalidClientId_ReturnsNotFound()
        {
            // Arrange
            var invalidClientId = 99999;

            // Act
            var response = await _client.PostAsync($"/api/payments/setup-intent/{invalidClientId}", null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            _output.WriteLine($"Invalid client ID {invalidClientId} correctly returned 404");
        }

        #endregion

        #region Save Payment Method Tests

        [Fact]
        public async Task SavePaymentMethod_ValidData_ReturnsSuccess()
        {
            // Arrange
            var clientId = await CreateTestClientAsync();

            var dto = new PaymentMethodCreateDto(
                ClientId: clientId,
                ProviderPaymentMethodId: StripeTestCards.Successful.Visa,
                Brand: "visa",
                Last4: "4242",
                ExpMonth: 12,
                ExpYear: 2028,
                IsDefault: true
            );

            // Act
            var response = await _client.PostAsJsonAsync("/api/payments/save-method", dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PaymentMethodDto>(content, JsonOptions.CamelCase);

            result.Should().NotBeNull();
            result.Brand.Should().Be("visa");
            result.Last4.Should().Be("4242");
            result.IsDefault.Should().BeTrue();

            _output.WriteLine($"Payment method saved via API: {result.Brand} ending in {result.Last4}");
        }

        [Fact]
        public async Task SavePaymentMethod_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var dto = new PaymentMethodCreateDto(
                ClientId: 0, // Invalid
                ProviderPaymentMethodId: "",
                Brand: "",
                Last4: "123", // Invalid length
                ExpMonth: 13, // Invalid
                ExpYear: 2020, // Past year
                IsDefault: true
            );

            // Act
            var response = await _client.PostAsJsonAsync("/api/payments/save-method", dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            _output.WriteLine("Invalid payment method data correctly returned 400");
        }

        #endregion

        #region Create Payment Tests

        [Theory]
        [InlineData(StripeTestCards.Successful.Visa, "succeeded", null)]
        [InlineData(StripeTestCards.Successful.Mastercard, "succeeded", null)]
        [InlineData(StripeTestCards.Declined.Generic, "error", "card_declined")]
        [InlineData(StripeTestCards.Declined.InsufficientFunds, "error", "insufficient_funds")]
        [InlineData(StripeTestCards.Declined.ExpiredCard, "error", "expired_card")]
        public async Task CreatePayment_DifferentTestCards_ReturnsExpectedResults(
            string paymentMethodId, string expectedStatus, string expectedErrorCode)
        {
            // Arrange
            var clientId = await CreateTestClientAsync();
            await CreateTestPaymentMethodAsync(clientId, paymentMethodId);

            var dto = new PaymentCreateDto(
                ClientId: clientId,
                PaymentMethodId: 1,
                ProviderPaymentMethodId: paymentMethodId,
                IdempotencyKey: $"test-payment-{Guid.NewGuid()}",
                AmountCents: 2500,
                Currency: "usd"
            );

            // Act
            var response = await _client.PostAsJsonAsync("/api/payments/charge", dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PaymentDto>(content, JsonOptions.CamelCase);

            result.Should().NotBeNull();
            result.Status.Should().Be(expectedStatus);
            result.ErrorCode.Should().Be(expectedErrorCode);
            result.AmountCents.Should().Be(2500);

            _output.WriteLine($"Payment test: {paymentMethodId} -> Status: {result.Status}, Error: {result.ErrorCode}");
        }

        [Fact]
        public async Task CreatePayment_AuthenticationRequired_ReturnsClientSecret()
        {
            // Arrange
            var clientId = await CreateTestClientAsync();
            await CreateTestPaymentMethodAsync(clientId, StripeTestCards.Authentication.Required);

            var dto = new PaymentCreateDto(
                ClientId: clientId,
                PaymentMethodId: 1,
                ProviderPaymentMethodId: StripeTestCards.Authentication.Required,
                IdempotencyKey: $"test-3ds-{Guid.NewGuid()}",
                AmountCents: 3000,
                Currency: "usd"
            );

            // Act
            var response = await _client.PostAsJsonAsync("/api/payments/charge", dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PaymentDto>(content, JsonOptions.CamelCase);

            result.Should().NotBeNull();
            result.Status.Should().Be("requires_action");
            result.ErrorCode.Should().Be("authentication_required");
            result.ClientSecret.Should().NotBeNullOrEmpty();
            result.ClientSecret.Should().StartWith("pi_");

            _output.WriteLine($"3DS payment: ClientSecret={result.ClientSecret}");
        }

        [Fact]
        public async Task CreatePayment_SameIdempotencyKey_ReturnsSameResult()
        {
            // Arrange
            var clientId = await CreateTestClientAsync();
            await CreateTestPaymentMethodAsync(clientId, StripeTestCards.Successful.Visa);

            var idempotencyKey = $"test-idempotency-{Guid.NewGuid()}";
            var dto = new PaymentCreateDto(
                ClientId: clientId,
                PaymentMethodId: 1,
                ProviderPaymentMethodId: StripeTestCards.Successful.Visa,
                IdempotencyKey: idempotencyKey,
                AmountCents: 1500,
                Currency: "usd"
            );

            // Act - First request
            var response1 = await _client.PostAsJsonAsync("/api/payments/charge", dto);
            var content1 = await response1.Content.ReadAsStringAsync();
            var result1 = JsonSerializer.Deserialize<PaymentDto>(content1, JsonOptions.CamelCase);

            // Act - Second request with same idempotency key
            var response2 = await _client.PostAsJsonAsync("/api/payments/charge", dto);
            var content2 = await response2.Content.ReadAsStringAsync();
            var result2 = JsonSerializer.Deserialize<PaymentDto>(content2, JsonOptions.CamelCase);

            // Assert
            response1.StatusCode.Should().Be(HttpStatusCode.OK);
            response2.StatusCode.Should().Be(HttpStatusCode.OK);
            result1.ProviderPaymentId.Should().Be(result2.ProviderPaymentId);

            _output.WriteLine($"Idempotent payment: {result1.ProviderPaymentId}");
        }

        #endregion

        #region Refund Tests

        [Fact]
        public async Task RefundPayment_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var clientId = await CreateTestClientAsync();
            await CreateTestPaymentMethodAsync(clientId, StripeTestCards.Successful.Visa);

            // Create a payment first
            var paymentDto = new PaymentCreateDto(
                ClientId: clientId,
                PaymentMethodId: 1,
                ProviderPaymentMethodId: StripeTestCards.Successful.Visa,
                IdempotencyKey: $"test-payment-{Guid.NewGuid()}",
                AmountCents: 5000,
                Currency: "usd"
            );

            var paymentResponse = await _client.PostAsJsonAsync("/api/payments/charge", paymentDto);
            paymentResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Get the payment ID (in real scenario, this would come from database)
            var paymentId = 1; // Assuming first payment gets ID 1

            var refundDto = new PaymentRefundDto(
                PaymentId: paymentId,
                AmountCents: 2000, // Partial refund
                IdempotencyKey: $"test-refund-{Guid.NewGuid()}"
            );

            // Act
            var response = await _client.PostAsJsonAsync("/api/payments/refund", refundDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PaymentRefundResultDto>(content, JsonOptions.CamelCase);

            result.Should().NotBeNull();
            result.Status.Should().Be("succeeded");
            result.AmountCents.Should().Be(2000);

            _output.WriteLine($"Refund processed: {result.ProviderRefundId}, Amount: ${result.AmountCents / 100.0:F2}");
        }

        #endregion

        #region International Currency Tests

        [Theory]
        [InlineData("eur", 2500)]
        [InlineData("gbp", 2000)]
        [InlineData("cad", 3000)]
        public async Task CreatePayment_InternationalCurrencies_ProcessesCorrectly(string currency, long amount)
        {
            // Arrange
            var clientId = await CreateTestClientAsync();
            await CreateTestPaymentMethodAsync(clientId, StripeTestCards.Successful.Visa);

            var dto = new PaymentCreateDto(
                ClientId: clientId,
                PaymentMethodId: 1,
                ProviderPaymentMethodId: StripeTestCards.Successful.Visa,
                IdempotencyKey: $"test-intl-{currency}-{Guid.NewGuid()}",
                AmountCents: amount,
                Currency: currency
            );

            // Act
            var response = await _client.PostAsJsonAsync("/api/payments/charge", dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PaymentDto>(content, JsonOptions.CamelCase);

            result.Should().NotBeNull();
            result.Currency.Should().Be(currency);
            result.AmountCents.Should().Be(amount);

            _output.WriteLine($"International payment: {currency.ToUpper()} {amount / 100.0:F2}");
        }

        #endregion

        #region Helper Methods

        private static Task<int> CreateTestClientAsync()
        {
            // This would typically create a test client in the database
            // For now, return a mock client ID
            // In a real implementation, you'd call your client creation endpoint
            return Task.FromResult(1);
        }

        private async Task CreateTestPaymentMethodAsync(int clientId, string paymentMethodId)
        {
            var dto = new PaymentMethodCreateDto(
                ClientId: clientId,
                ProviderPaymentMethodId: paymentMethodId,
                Brand: StripeTestCards.GetBrandForTestCard(paymentMethodId),
                Last4: StripeTestCards.GetLast4ForTestCard(paymentMethodId),
                ExpMonth: 12,
                ExpYear: 2028,
                IsDefault: true
            );

            var response = await _client.PostAsJsonAsync("/api/payments/save-method", dto);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        #endregion

        public void Dispose()
        {
            _client?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
