using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Stripe;
using UcarMobileApi.Application.Mapping;
using UcarMobileApi.Infrastructure.Data;
using UcarMobileApi.Tests.TestHelpers;

namespace UcarMobileApi.Tests.Integration.Payments
{
    /// <summary>
    /// Test fixture for Stripe integration tests
    /// Configures test environment and provides shared resources
    /// </summary>
    public class StripeTestFixture : IDisposable
    {
        public IMapper Mapper { get; }
        public IConfiguration Configuration { get; }

        public StripeTestFixture()
        {
            // Configure Stripe for testing
            ConfigureStripeForTesting();

            // Setup AutoMapper
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PaymentMappingProfile>();
            });
            Mapper = mapperConfig.CreateMapper();

            // Setup configuration
            Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Stripe:SecretKey", "" },
                    { "Stripe:PublishableKey", "pk_test_51SKOBK3YV7wtFxWKPiVWNdMSLSPeieWHx1ZpM9FeqzkH3nzR1McleuoQUFrhKoxkUOqYgT3Ob4VQhbRrujXaxuSR00ILTIcNZq" },
                    { "Stripe:WebhookSecret", "whsec_abcdef1234567890" }
                })
                .Build();
        }

        private static void ConfigureStripeForTesting()
        {
            // Set Stripe test key - use your actual test key or mock
            StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("STRIPE_TEST_SECRET_KEY")
                ?? "sk_test_51234567890"; // Replace with your test key

            // Configure Stripe for test mode
            StripeConfiguration.MaxNetworkRetries = 2;
        }

        public AppDbContext CreateDbContext()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = TestDbContextFactory.CreateInMemoryContext(dbName);
            context.Database.EnsureCreated();
            return context;
        }

        public void Dispose()
        {
            // Cleanup if needed
            GC.SuppressFinalize(this);
        }
    }
}
