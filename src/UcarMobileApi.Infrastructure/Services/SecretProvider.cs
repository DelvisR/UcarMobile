using System;
using System.Threading.Tasks;
using Amazon.SecretsManager.Extensions.Caching;
using Microsoft.Extensions.Logging;
using UcarMobileApi.Application.Common.Interfaces;

namespace UcarMobileApi.Infrastructure.Services;

/// <summary>
/// Service for retrieving secrets from AWS Secrets Manager with caching and comprehensive error handling
/// </summary>
/// <remarks>
/// Initializes a new instance of the SecretProvider class
/// </remarks>
/// <param name="cache">The AWS Secrets Manager cache instance</param>
/// <param name="logger">The logger instance (ILogger&lt;SecretProvider&gt;)</param>
/// <exception cref="ArgumentNullException">Thrown when cache or logger is null</exception>
public class SecretProvider(ISecretsManagerCache cache, ILogger<SecretProvider> logger) : ISecretProvider
{
    /// <summary>
    /// Retrieves a secret value from AWS Secrets Manager with comprehensive error handling
    /// </summary>
    /// <param name="secretName">The name of the secret to retrieve</param>
    /// <returns>The secret value if successfully retrieved, null in case of any error</returns>
    public async Task<string?> GetSecretAsync(string secretName)
    {
        // Validate input parameter
        if (string.IsNullOrWhiteSpace(secretName))
        {
            logger.LogWarning("Secret name is null or empty");
            return null;
        }

        try
        {
            // Retrieve secret from AWS Secrets Manager cache
            var secretValue = await cache.GetSecretString(secretName);

            // Check if secret value is empty or null
            if (!string.IsNullOrEmpty(secretValue)) return secretValue;

            logger.LogWarning("Secret '{SecretName}' was found but returned null or empty value", secretName);
            return null;

        }
        catch (Amazon.SecretsManager.Model.ResourceNotFoundException ex)
        {
            logger.LogError(ex, "Secret '{SecretName}' not found in AWS Secrets Manager", secretName);
            return null;
        }
        catch (Amazon.SecretsManager.Model.InvalidRequestException ex)
        {
            logger.LogError(ex, "Invalid request when retrieving secret '{SecretName}': {ErrorMessage}", secretName, ex.Message);
            return null;
        }
        catch (Amazon.SecretsManager.Model.InvalidParameterException ex)
        {
            logger.LogError(ex, "Invalid parameter when retrieving secret '{SecretName}': {ErrorMessage}", secretName, ex.Message);
            return null;
        }
        catch (Amazon.SecretsManager.Model.DecryptionFailureException ex)
        {
            logger.LogError(ex, "Decryption failed for secret '{SecretName}': {ErrorMessage}", secretName, ex.Message);
            return null;
        }
        catch (Amazon.SecretsManager.Model.InternalServiceErrorException ex)
        {
            logger.LogError(ex, "AWS Secrets Manager internal service error when retrieving secret '{SecretName}': {ErrorMessage}", secretName, ex.Message);
            return null;
        }
        catch (Amazon.Runtime.AmazonServiceException ex)
        {
            logger.LogError(ex, "AWS service error when retrieving secret '{SecretName}': {ErrorCode} - {ErrorMessage}", secretName, ex.ErrorCode, ex.Message);
            return null;
        }
        catch (Amazon.Runtime.AmazonClientException ex)
        {
            logger.LogError(ex, "AWS client error when retrieving secret '{SecretName}': {ErrorMessage}", secretName, ex.Message);
            return null;
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogError(ex, "Unauthorized access when retrieving secret '{SecretName}': {ErrorMessage}", secretName, ex.Message);
            return null;
        }
        catch (TimeoutException ex)
        {
            logger.LogError(ex, "Timeout occurred when retrieving secret '{SecretName}': {ErrorMessage}", secretName, ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error occurred when retrieving secret '{SecretName}': {ErrorMessage}", secretName, ex.Message);
            return null;
        }
    }
}
