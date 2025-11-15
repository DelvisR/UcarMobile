using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Infrastructure.Configurations.Settings;

namespace UcarMobileApi.Infrastructure.Providers;

/// <summary>
/// Provides access to the Google API key, retrieving it from configuration or AWS Secrets Manager.
/// Implements lazy loading and caching to prevent redundant lookups.
/// </summary>
/// <remarks>
/// This provider ensures that the Google API key is loaded only once during the application lifetime.
/// If the key is defined in configuration (appsettings, environment variables, etc.), it will use that.
/// Otherwise, it attempts to retrieve the key from AWS Secrets Manager.
/// </remarks>
public class GoogleApiKeyProvider
{
    private readonly Lazy<Task<string>> _apiKeyLoader;

    /// <summary>
    /// Initializes a new instance of the <see cref="GoogleApiKeyProvider"/> class.
    /// </summary>
    /// <param name="secretProvider">An abstraction for accessing secrets from external sources such as AWS Secrets Manager.</param>
    /// <param name="configuration">The application configuration source (used for reading static API keys).</param>
    /// <param name="awsOptions">The AWS configuration options that include secret identifiers.</param>
    public GoogleApiKeyProvider(ISecretProvider secretProvider, IConfiguration configuration, IOptions<AwsSettings> awsOptions)
    {
        var awsSettings = awsOptions.Value;
        _apiKeyLoader = new Lazy<Task<string>>(() => LoadApiKeyAsync(secretProvider, configuration, awsSettings));
    }

    /// <summary>
    /// Retrieves the Google API key, using cached value if already loaded.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the Google API key.</returns>
    public Task<string> GetGoogleApiKeyAsync() => _apiKeyLoader.Value;

    /// <summary>
    /// Loads the Google API key from configuration or AWS Secrets Manager.
    /// </summary>
    /// <param name="secretProvider">The secret provider used to access AWS Secrets Manager.</param>
    /// <param name="configuration">The configuration source for static API keys.</param>
    /// <param name="awsSettings">The AWS settings containing the secret identifier.</param>
    /// <returns>A task representing the asynchronous operation, containing the loaded API key.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the API key cannot be found in any source.</exception>
    private static async Task<string> LoadApiKeyAsync(ISecretProvider secretProvider, IConfiguration configuration, AwsSettings awsSettings)
    {
        // Try to get API key directly from configuration
        var apiKey = configuration["Google:ApiKey"];
        if (!string.IsNullOrEmpty(apiKey))
        {
            return apiKey;
        }

        // Fallback to AWS Secrets Manager
        var secretId = awsSettings.Secrets?.GoogleApiKeySecretId
                       ?? throw new InvalidOperationException("AWS GoogleApiKeySecretId is missing from configuration.");

        var secretValue = await secretProvider.GetSecretAsync(secretId)
                          ?? throw new InvalidOperationException($"AWS Secret '{secretId}' returned empty value.");

        // Try to parse JSON secret structure
        try
        {
            var secretJson = JsonSerializer.Deserialize<Dictionary<string, string>>(secretValue);
            if (secretJson is not null && secretJson.TryGetValue("api-key", out var parsedKey))
            {
                return parsedKey;
            }
        }
        catch (JsonException)
        {
            // Ignore JSON parsing error and treat the secret as plain text.
        }

        // Fallback to raw value (if the secret was stored as a plain string)
        return secretValue;
    }
}
