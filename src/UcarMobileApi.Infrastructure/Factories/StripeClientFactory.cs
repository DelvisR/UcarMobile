using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Stripe;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Infrastructure.Configurations.Settings;

namespace UcarMobileApi.Infrastructure.Factories
{
    /// <summary>
    /// Provides initialization and credential management for the Stripe SDK.
    /// Retrieves and caches Stripe credentials from configuration or AWS Secrets Manager.
    /// </summary>
    public class StripeClientFactory(ISecretProvider secretProvider, IOptions<AwsSettings> awsOptions, IConfiguration configuration)
    {
        private readonly Lazy<Task<(string secretKey, string webhookSecret)>> _lazyStripeSecrets = new(() => LoadStripeSecretsAsync(secretProvider, awsOptions, configuration));

        /// <summary>
        /// Retrieves the Stripe webhook secret asynchronously (cached after first retrieval).
        /// </summary>
        public async Task<string> GetWebhookSecretAsync()
        {
            var (_, webhookSecret) = await _lazyStripeSecrets.Value;
            return webhookSecret;
        }

        /// <summary>
        /// Initializes the global Stripe SDK configuration.
        /// </summary>
        public async Task InitializeStripeAsync()
        {
            var (secretKey, webhookSecret) = await _lazyStripeSecrets.Value;

            StripeConfiguration.ApiKey = secretKey;

            if (string.IsNullOrWhiteSpace(webhookSecret))
                throw new InvalidOperationException("Stripe Webhook Secret is missing. Please verify configuration or AWS Secrets Manager.");
        }

        /// <summary>
        /// Loads Stripe credentials from configuration or AWS Secrets Manager.
        /// </summary>
        private static async Task<(string secretKey, string webhookSecret)> LoadStripeSecretsAsync(ISecretProvider secretProvider, IOptions<AwsSettings> awsOptions, IConfiguration configuration)
        {
            // 1️ Try configuration first
            var secretKey = configuration["Stripe:SecretKey"]?.Trim();
            var webhookSecret = configuration["Stripe:WebhookSecret"]?.Trim();

            if (!string.IsNullOrWhiteSpace(secretKey) && !string.IsNullOrWhiteSpace(webhookSecret))
                return (secretKey, webhookSecret);

            // 2️ Try AWS Secrets Manager
            var secretId = awsOptions.Value.Secrets?.StripeSecretId
                ?? throw new InvalidOperationException("AWS StripeSecretId is missing from configuration.");

            var secretValue = await secretProvider.GetSecretAsync(secretId)
                ?? throw new InvalidOperationException($"AWS Secret '{secretId}' returned empty value.");

            string? awsSecretKey = null;
            string? awsWebhookSecret = null;

            try
            {
                var doc = JsonDocument.Parse(secretValue);
                if (doc.RootElement.TryGetProperty("STRIPE_SECRET_KEY", out var sKey))
                    awsSecretKey = sKey.GetString()?.Trim();
                if (doc.RootElement.TryGetProperty("STRIPE_WEBHOOK_SECRET", out var wKey))
                    awsWebhookSecret = wKey.GetString()?.Trim();
            }
            catch (JsonException)
            {
                awsSecretKey = secretValue.Trim(); // fallback for raw string
            }

            if (string.IsNullOrWhiteSpace(awsSecretKey))
                throw new InvalidOperationException("Stripe Secret Key was not found in configuration or AWS.");

            if (string.IsNullOrWhiteSpace(awsWebhookSecret))
                throw new InvalidOperationException("Stripe Webhook Secret was not found in configuration or AWS.");

            return (awsSecretKey, awsWebhookSecret);
        }
    }
}
