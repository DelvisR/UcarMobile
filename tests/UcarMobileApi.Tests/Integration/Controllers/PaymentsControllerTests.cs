using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using UcarMobileApi.Application.DTOs.Payments;
using UcarMobileApi.Core.Entities.Clients;
using UcarMobileApi.Core.Entities.Users;
using UcarMobileApi.Infrastructure.Data;
using UcarMobileApi.Tests.TestHelpers;
using UcarMobileApi.Tests.Unit.Payments;
using Xunit;
using Xunit.Abstractions;

namespace UcarMobileApi.Tests.Integration.Controllers
{
    /// <summary>
    /// Integration tests for PaymentsController
    /// Tests HTTP endpoints with various Stripe test scenarios
    /// </summary>
    public class PaymentsControllerTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;
        private const string TestAuthProviderId = "test-client-cognito-id-123";
        private const string TestDbName = "PaymentsTestDb";

        public PaymentsControllerTests(WebApplicationFactory<Program> factory, ITestOutputHelper output)
        {
            _output = output;

            // Configuración de la fábrica para in-memory DB y seed de datos
            var factory1 = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseInMemoryDatabase(TestDbName);
                    });

                    var sp = services.BuildServiceProvider();
                    using var scope = sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    SeedTestData(db);
                });
            });

            _client = factory1.CreateClient();
        }

        public static class JsonOptions
        {
            public static readonly JsonSerializerOptions CamelCase = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        #region Setup Intent Tests

        [Fact]
        public async Task CreateSetupIntent_WithAuthenticatedUser_ReturnsSuccess()
        {
            await AuthenticateAsTestClientAsync();

            var response = await _client.PostAsync("/api/payments/setup-intent", null);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PaymentSetupDto>(content, JsonOptions.CamelCase);

            result.Should().NotBeNull();
            result!.ClientSecret.Should().StartWith("seti_");
            result.ProviderCustomerId.Should().StartWith("cus_");

            _output.WriteLine($"SetupIntent created via API: {result.ClientSecret}");
        }

        [Fact]
        public async Task CreateSetupIntent_WithoutAuthentication_ReturnsUnauthorized()
        {
            var response = await _client.PostAsync("/api/payments/setup-intent", null);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            _output.WriteLine("Setup intent request without authentication correctly returned 401");
        }

        #endregion

        #region Attach Payment Method Tests

        [Fact]
        public async Task AttachPaymentMethod_ValidData_ReturnsSuccess()
        {
            await AuthenticateAsTestClientAsync();

            var dto = new PaymentMethodAttachDto(
                ProviderPaymentMethodId: StripeTestCards.Successful.Visa,
                IsDefault: true
            );

            var response = await _client.PostAsJsonAsync("/api/payments/attach-method", dto);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PaymentMethodDto>(content, JsonOptions.CamelCase);

            result.Should().NotBeNull();
            result!.Brand.Should().Be("visa");
            result.Last4.Should().Be("4242");
            result.IsDefault.Should().BeTrue();

            _output.WriteLine($"Payment method attached via API: {result.Brand} ending in {result.Last4}");
        }

        [Fact]
        public async Task AttachPaymentMethod_InvalidData_ReturnsBadRequest()
        {
            await AuthenticateAsTestClientAsync();

            var dto = new PaymentMethodAttachDto(
                ProviderPaymentMethodId: "", // Invalid
                IsDefault: true
            );

            var response = await _client.PostAsJsonAsync("/api/payments/attach-method", dto);

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
            await AuthenticateAsTestClientAsync();
            await AttachTestPaymentMethodAsync(paymentMethodId);

            var dto = new PaymentCreateDto(
                PaymentMethodId: 1,
                IdempotencyKey: $"test-payment-{Guid.NewGuid()}",
                AmountCents: 2500,
                Currency: "usd"
            );

            var response = await _client.PostAsJsonAsync("/api/payments/charge", dto);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PaymentDto>(content, JsonOptions.CamelCase);

            result.Should().NotBeNull();
            result!.Status.Should().Be(expectedStatus);
            result.ErrorCode.Should().Be(expectedErrorCode);
            result.AmountCents.Should().Be(2500);

            _output.WriteLine($"Payment test: {paymentMethodId} -> Status: {result.Status}, Error: {result.ErrorCode}");
        }

        [Fact]
        public async Task CreatePayment_AuthenticationRequired_ReturnsClientSecret()
        {
            await AuthenticateAsTestClientAsync();
            await AttachTestPaymentMethodAsync(StripeTestCards.Authentication.Required);

            var dto = new PaymentCreateDto(
                PaymentMethodId: 1,
                IdempotencyKey: $"test-3ds-{Guid.NewGuid()}",
                AmountCents: 3000,
                Currency: "usd"
            );

            var response = await _client.PostAsJsonAsync("/api/payments/charge", dto);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PaymentDto>(content, JsonOptions.CamelCase);

            result.Should().NotBeNull();
            result!.Status.Should().Be("requires_action");
            result.ErrorCode.Should().Be("authentication_required");
            result.ClientSecret.Should().NotBeNullOrEmpty();
            result.ClientSecret.Should().StartWith("pi_");

            _output.WriteLine($"3DS payment: ClientSecret={result.ClientSecret}");
        }

        [Fact]
        public async Task CreatePayment_SameIdempotencyKey_ReturnsSameResult()
        {
            await AuthenticateAsTestClientAsync();
            await AttachTestPaymentMethodAsync(StripeTestCards.Successful.Visa);

            var idempotencyKey = $"test-idempotency-{Guid.NewGuid()}";
            var dto = new PaymentCreateDto(
                PaymentMethodId: 1,
                IdempotencyKey: idempotencyKey,
                AmountCents: 1500,
                Currency: "usd"
            );

            var response1 = await _client.PostAsJsonAsync("/api/payments/charge", dto);
            var content1 = await response1.Content.ReadAsStringAsync();
            var result1 = JsonSerializer.Deserialize<PaymentDto>(content1, JsonOptions.CamelCase);

            var response2 = await _client.PostAsJsonAsync("/api/payments/charge", dto);
            var content2 = await response2.Content.ReadAsStringAsync();
            var result2 = JsonSerializer.Deserialize<PaymentDto>(content2, JsonOptions.CamelCase);

            response1.StatusCode.Should().Be(HttpStatusCode.OK);
            response2.StatusCode.Should().Be(HttpStatusCode.OK);
            result1!.ProviderPaymentId.Should().Be(result2!.ProviderPaymentId);

            _output.WriteLine($"Idempotent payment: {result1.ProviderPaymentId}");
        }

        #endregion

        #region Refund Tests

        [Fact]
        public async Task RefundPayment_ValidRequest_ReturnsSuccess()
        {
            await AuthenticateAsTestClientAsync();
            await AttachTestPaymentMethodAsync(StripeTestCards.Successful.Visa);

            var paymentDto = new PaymentCreateDto(
                PaymentMethodId: 1,
                IdempotencyKey: $"test-payment-{Guid.NewGuid()}",
                AmountCents: 5000,
                Currency: "usd"
            );

            var paymentResponse = await _client.PostAsJsonAsync("/api/payments/charge", paymentDto);
            paymentResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var paymentId = 1; // Primer pago

            var refundDto = new PaymentRefundDto(
                PaymentId: paymentId,
                AmountCents: 2000,
                IdempotencyKey: $"test-refund-{Guid.NewGuid()}"
            );

            var response = await _client.PostAsJsonAsync("/api/payments/refund", refundDto);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PaymentRefundResultDto>(content, JsonOptions.CamelCase);

            result.Should().NotBeNull();
            result!.Status.Should().Be("succeeded");
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
            await AuthenticateAsTestClientAsync();
            await AttachTestPaymentMethodAsync(StripeTestCards.Successful.Visa);

            var dto = new PaymentCreateDto(
                PaymentMethodId: 1,
                IdempotencyKey: $"test-intl-{currency}-{Guid.NewGuid()}",
                AmountCents: amount,
                Currency: currency
            );

            var response = await _client.PostAsJsonAsync("/api/payments/charge", dto);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PaymentDto>(content, JsonOptions.CamelCase);

            result.Should().NotBeNull();
            result!.Currency.Should().Be(currency);
            result.AmountCents.Should().Be(amount);

            _output.WriteLine($"International payment: {currency.ToUpper()} {amount / 100.0:F2}");
        }

        #endregion

        #region Helper Methods

        private static void SeedTestData(AppDbContext db)
        {
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var clientRole = new Role
            {
                Id = 1,
                Name = "Client",
                Description = "Test client role",
                CreatedBy = "system",
                CreatedDate = DateTime.UtcNow,
                LastModifiedBy = "system",
                LastModifiedDate = DateTime.UtcNow
            };
            db.Set<Role>().Add(clientRole);

            var testClient = new Client
            {
                Id = 1,
                AuthProviderId = TestAuthProviderId,
                Email = "test.client@example.com",
                Phone = "1234567890",
                FirstName = "Test",
                LastName = "Client",
                Address = "123 Test Street",
                IsActive = true,
                CreatedBy = "system",
                CreatedDate = DateTime.UtcNow,
                LastModifiedBy = "system",
                LastModifiedDate = DateTime.UtcNow
            };
            db.Set<Client>().Add(testClient);

            var userRole = new UserRole
            {
                UserId = 1,
                RoleId = 1,
                CreatedBy = "system",
                CreatedDate = DateTime.UtcNow,
                LastModifiedBy = "system",
                LastModifiedDate = DateTime.UtcNow
            };
            db.Set<UserRole>().Add(userRole);

            db.SaveChanges();
        }

        private Task AuthenticateAsTestClientAsync()
        {
            var token = JwtTestTokenGenerator.GenerateToken(TestAuthProviderId);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return Task.CompletedTask;
        }

        private async Task AttachTestPaymentMethodAsync(string paymentMethodId)
        {
            var dto = new PaymentMethodAttachDto(
                ProviderPaymentMethodId: paymentMethodId,
                IsDefault: true
            );

            var response = await _client.PostAsJsonAsync("/api/payments/attach-method", dto);
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
