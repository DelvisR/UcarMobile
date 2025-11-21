using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Stripe;
using UcarMobileApi.Application;
using UcarMobileApi.Application.DTOs;
using UcarMobileApi.Application.Services.Security;
using UcarMobileApi.Core.Entities.Clients;
using UcarMobileApi.Core.Entities.Payments;
using UcarMobileApi.Infrastructure.Data;
using UcarMobileApi.Infrastructure.Services.Payments;
using UcarMobileApi.Tests.TestHelpers;
using Xunit;
using PaymentMethod = UcarMobileApi.Core.Entities.Payments.PaymentMethod;

namespace UcarMobileApi.Tests.Integration.Payments
{
    public class StripePaymentServiceTests
    {
        private readonly AppDbContext _db;
        private readonly Mock<ICacheService> _cache = new();
        private readonly Mock<ILogger<StripePaymentService>> _logger = new();
        private readonly IMapper _mapper;

        private const string AuthProviderId = "test-client-auth-id";

        public StripePaymentServiceTests()
        {
            _db = TestDbContextFactory.CreateInMemoryContext(Guid.NewGuid().ToString());

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(ApplicationAssemblyMarker).Assembly);
            });

            _mapper = config.CreateMapper();

            SeedClient();
        }

        private void SeedClient()
        {
            var client = new Client
            {
                Id = 1,
                AuthProviderId = AuthProviderId,
                Email = "client@test.com",
                FirstName = "Test",
                LastName = "Client",
                Phone = "5551112233",
                IsActive = true,
                ProviderPaymentCustomerId = null
            };

            _db.Set<Client>().Add(client);
            _db.SaveChanges();
        }

        #region Setup Intent

        [Fact]
        public async Task CreateSetupIntent_ReturnsClientSecretAndCustomerId()
        {
            // Mock Stripe SetupIntentService
            var stripeMock = new Mock<SetupIntentService>();
            stripeMock
                .Setup(m => m.CreateAsync(
                    It.IsAny<SetupIntentCreateOptions>(),
                    null,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SetupIntent
                {
                    ClientSecret = "seti_test_123"
                });

            // Mock Stripe CustomerService → simula creación de cliente
            var customerMock = new Mock<CustomerService>();
            customerMock
                .Setup(c => c.CreateAsync(
                    It.IsAny<CustomerCreateOptions>(),
                    null,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Customer
                {
                    Id = "cus_test_987"
                });

            var service = ServiceFactory(o =>
            {
                o.SetupIntentService = stripeMock.Object;
                o.CustomerService = customerMock.Object;
            });

            // Act
            var result = await service.CreateSetupIntentAsync(AuthProviderId);

            // Assert
            result.ClientSecret.Should().Be("seti_test_123");
            result.ProviderCustomerId.Should().Be("cus_test_987");

            // Verificar que guardó el CustomerId en la BD
            var client = await _db.Set<Client>().FirstAsync(c => c.AuthProviderId == AuthProviderId);
            client.ProviderPaymentCustomerId.Should().Be("cus_test_987");
        }

        #endregion

        #region Attach Payment Method

        [Fact]
        public async Task AttachPaymentMethod_SavesToDatabase_ReturnsDto()
        {
            // Mock PaymentMethodService.AttachAsync
            var paymentMethodMock = new Mock<PaymentMethodService>();
            paymentMethodMock
                .Setup(m => m.AttachAsync(
                    It.IsAny<string>(),
                    It.IsAny<PaymentMethodAttachOptions>(),
                    null,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Stripe.PaymentMethod
                {
                    Id = "pm_123",
                    Card = new PaymentMethodCard
                    {
                        Brand = "visa",
                        Last4 = "4242",
                        ExpMonth = 12,
                        ExpYear = 2030
                    }
                });

            // Mock CustomerService (por si el cliente no tiene StripeCustomerId)
            var customerMock = new Mock<CustomerService>();
            customerMock
                .Setup(c => c.CreateAsync(
                    It.IsAny<CustomerCreateOptions>(),
                    null,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Customer
                {
                    Id = "cus_999"
                });

            var service = ServiceFactory(o =>
            {
                o.PaymentMethodService = paymentMethodMock.Object;
                o.CustomerService = customerMock.Object;
            });

            var dto = new PaymentMethodAttachDto("pm_123", true);

            // Act
            var result = await service.AttachPaymentMethodAsync(AuthProviderId, dto);

            // Assert (DTO)
            result.Should().NotBeNull();
            result.Brand.Should().Be("visa");
            result.Last4.Should().Be("4242");
            result.IsDefault.Should().BeTrue();

            // Assert (DB)
            var stored = await _db.Set<PaymentMethod>().FirstOrDefaultAsync();
            stored.Should().NotBeNull();
            stored.ProviderPaymentMethodId.Should().Be("pm_123");
            stored.Brand.Should().Be("visa");
            stored.Last4.Should().Be("4242");
            stored.IsDefault.Should().BeTrue();
        }

        #endregion

        #region Payments

        [Fact]
        public async Task CreatePayment_Successful_ReturnsProviderIdAndStatus()
        {
            var stripeMock = new Mock<PaymentIntentService>();
            stripeMock
                .Setup(m => m.CreateAsync(
                    It.IsAny<PaymentIntentCreateOptions>(),
                    null,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PaymentIntent
                {
                    Id = "pi_123",
                    Amount = 2500,
                    Status = "succeeded",
                    Currency = "usd"
                });

            var service = ServiceFactory(o =>
            {
                o.PaymentIntentService = stripeMock.Object;
            });

            var dto = new PaymentCreateDto(
                PaymentMethodId: 1,
                IdempotencyKey: "test-idemp-1",
                AmountCents: 2500,
                Currency: "usd"
            );

            var result = await service.CreatePaymentAsync(AuthProviderId, dto);

            result.Status.Should().Be("succeeded");
            result.ProviderPaymentId.Should().Be("pi_123");
            result.AmountCents.Should().Be(2500);
        }

        [Fact]
        public async Task CreatePayment_StripeDecline_ReturnsErrorStatus()
        {
            var stripeMock = new Mock<PaymentIntentService>();
            stripeMock
                .Setup(m => m.CreateAsync(
                    It.IsAny<PaymentIntentCreateOptions>(),
                    null,
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new StripeException
                {
                    StripeError = new StripeError { Code = "card_declined" }
                });

            var service = ServiceFactory(o =>
            {
                o.PaymentIntentService = stripeMock.Object;
            });

            var dto = new PaymentCreateDto(
                1, "pm_declined", 1500);

            var client = await _db.Set<Client>().FirstAsync(c => c.AuthProviderId == AuthProviderId);
            client.ProviderPaymentCustomerId = "cus_test_decline";
            await _db.SaveChangesAsync();

            var result = await service.CreatePaymentAsync(AuthProviderId, dto);

            result.Status.Should().Be("error");
            result.ErrorCode.Should().Be("card_declined");
        }

        #endregion

        #region Refunds

        [Fact]
        public async Task RefundPayment_Success_ReturnsRefundDto()
        {
            // Seed del pago
            var payment = new Payment
            {
                Id = 1,
                ProviderPaymentId = "pi_abc",
                AmountCents = 5000,
                Currency = "usd",
                Status = "succeeded",
                ClientId = 1
            };
            await _db.Set<Payment>().AddAsync(payment);
            await _db.SaveChangesAsync();

            var stripeMock = new Mock<RefundService>();
            stripeMock
                .Setup(m => m.CreateAsync(
                    It.IsAny<RefundCreateOptions>(),
                    null,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Refund
                {
                    Id = "re_123",
                    Amount = 2000,
                    Status = "succeeded",
                    Currency = "usd"
                });

            var service = ServiceFactory(o =>
            {
                o.RefundService = stripeMock.Object;
            });

            var dto = new PaymentRefundDto(
                PaymentId: 1,
                AmountCents: 2000,
                IdempotencyKey: "refund-1"
            );

            var result = await service.RefundPaymentAsync(dto);

            result.ProviderRefundId.Should().Be("re_123");
            result.Status.Should().Be("succeeded");
            result.AmountCents.Should().Be(2000);
        }

        #endregion

        #region Factory

        private StripePaymentService ServiceFactory(Action<StripeOverrides> overrides = null)
        {
            var o = new StripeOverrides();
            overrides?.Invoke(o);

            return new StripePaymentService(
                _db,
                _mapper,
                _logger.Object,
                _cache.Object,
                o.CustomerService,
                o.SetupIntentService,
                o.PaymentMethodService,
                o.PaymentIntentService,
                o.RefundService
            );
        }

        private class StripeOverrides
        {
            public CustomerService CustomerService { get; set; } = Mock.Of<CustomerService>();
            public SetupIntentService SetupIntentService { get; set; } = Mock.Of<SetupIntentService>();
            public PaymentMethodService PaymentMethodService { get; set; } = Mock.Of<PaymentMethodService>();
            public PaymentIntentService PaymentIntentService { get; set; } = Mock.Of<PaymentIntentService>();
            public RefundService RefundService { get; set; } = Mock.Of<RefundService>();
        }

        #endregion
    }
}
