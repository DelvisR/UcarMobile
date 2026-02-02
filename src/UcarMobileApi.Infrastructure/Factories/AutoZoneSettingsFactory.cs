using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Infrastructure.Configurations.AutoZone.Models;
using UcarMobileApi.Infrastructure.Configurations.Settings;

namespace UcarMobileApi.Infrastructure.Factories
{
    public class AutoZoneSettingsFactory(ISecretProvider secretProvider, IOptions<AwsSettings> awsOptions,
         ILogger<AutoZoneSettingsFactory> logger, AutoZoneSettingsDto options)
    {

        /// <summary>
        /// Configures the application settings required for AutoZone integration.
        /// </summary>
        /// <remarks>This method loads necessary secrets and configuration values for AutoZone from AWS
        /// Secrets Manager. It should be called before performing operations that depend on AutoZone
        /// settings.</remarks>
        /// <exception cref="InvalidOperationException">Thrown if an error occurs while configuring AutoZone settings. See the inner exception for details.</exception>
        public async Task ConfigureAutoZoneSettings()
        {
            try
            {
                logger.LogInformation("Configuring AutoZone settings");
                await LoadSecretsFromAwsSecretsManagerAsync(options);

                logger.LogInformation("AutoZone settings configured successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to configure AutoZone settings");
                throw new InvalidOperationException(
                    "Error configuring AutoZone settings. See inner exception for details.", ex);
            }

        }

        private async Task LoadSecretsFromAwsSecretsManagerAsync(AutoZoneSettingsDto options)
        {
            var awsSecretName = GetAwsSecretName();

            logger.LogInformation("Retrieving AutoZone secrets from AWS Secrets Manager: {SecretName}", awsSecretName);

            var secretJsonContent = await RetrieveSecretFromAwsAsync(awsSecretName);
            var parsedSecrets = ParseAwsSecretJson(secretJsonContent, awsSecretName);

            ValidateAwsSecrets(parsedSecrets);

            options.ClientId = parsedSecrets.ClientId!;
            options.ClientSecret = parsedSecrets.ClientSecret!;
            options.Service = parsedSecrets.Service!;
            options.ApiBaseUrl = parsedSecrets.ApiBaseUrl!;
            options.OAuthUrl = parsedSecrets.OAuthUrl!;
            options.AuthScope = parsedSecrets.AuthScope!;

            logger.LogInformation("AutoZone secrets loaded successfully from AWS Secrets Manager");
        }

        private string GetAwsSecretName()
        {
            return awsOptions.Value.Secrets?.AutoZoneSecrets
                ?? throw new InvalidOperationException(
                    "AWS:Secrets:AutoZoneSecrets is missing from configuration.");
        }

        private async Task<string> RetrieveSecretFromAwsAsync(string secretName)
        {
            var secretValue = await secretProvider.GetSecretAsync(secretName);

            if (string.IsNullOrWhiteSpace(secretValue))
            {
                throw new InvalidOperationException(
                    $"AWS Secret '{secretName}' returned empty or null value.");
            }

            return secretValue;
        }

        private (string? ClientId, string? ClientSecret, string? Service,
            string? ApiBaseUrl, string? OAuthUrl, string? AuthScope) ParseAwsSecretJson(string jsonContent, string secretName)
        {
            try
            {
                using var jsonDocument = JsonDocument.Parse(jsonContent);
                var root = jsonDocument.RootElement;

                var clientId = TryGetJsonProperty(root, "username");
                var clientSecret = TryGetJsonProperty(root, "password");
                var service = TryGetJsonProperty(root, "service");
                var apiBaseUrl = TryGetJsonProperty(root, "apiBaseUrl");
                var oAuthUrl = TryGetJsonProperty(root, "oAuthUrl");
                var authScope = TryGetJsonProperty(root, "authScope");

                return (clientId, clientSecret, service, apiBaseUrl, oAuthUrl, authScope);
            }
            catch (JsonException ex)
            {
                logger.LogError(ex, "Failed to parse AutoZone secret as JSON from AWS");
                throw new InvalidOperationException(
                    $"AWS Secret '{secretName}' contains invalid JSON format.", ex);
            }
        }

        private void ValidateAwsSecrets((string? ClientId, string? ClientSecret, string? Service, string? apiBaseUrl,
            string? oAuthUrl, string? authScope) secrets)
        {
            if (string.IsNullOrWhiteSpace(secrets.ClientId))
            {
                throw new InvalidOperationException(
                    "AutoZone ClientId (username) was not found in AWS Secrets Manager.");
            }

            if (string.IsNullOrWhiteSpace(secrets.ClientSecret))
            {
                throw new InvalidOperationException(
                    "AutoZone ClientSecret (password) was not found in AWS Secrets Manager.");
            }

            if (string.IsNullOrWhiteSpace(secrets.Service))
            {
                throw new InvalidOperationException(
                    "AutoZone Service was not found in AWS Secrets Manager.");
            }

            if (string.IsNullOrWhiteSpace(secrets.apiBaseUrl) ||
                string.IsNullOrWhiteSpace(secrets.oAuthUrl) ||
                string.IsNullOrWhiteSpace(secrets.authScope))
            {
                throw new InvalidOperationException(
                    "One or more required AutoZone secrets are missing from AWS Secrets Manager.");
            }
        }


        private string? TryGetJsonProperty(JsonElement element, string propertyName)
        {
            return element.TryGetProperty(propertyName, out var property)
                ? property.GetString()?.Trim()
                : null;
        }
    }
}
