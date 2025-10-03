using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using UcarMobileApi.Application.Common;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Infrastructure.Configurations.Settings;
using UcarMobileApi.Infrastructure.Data;

namespace UcarMobileApi.Infrastructure.Services;

/// <summary>
/// Provides a central factory to construct a database connection string.
/// It first tries to get the string from appsettings/connectionstrings,
/// then falls back to AWS Secrets Manager if not found.
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Returns the database connection string asynchronously.
    /// </summary>
    Task<string> GetConnectionStringAsync();
}

/// <summary>
/// Default implementation of <see cref="IDbConnectionFactory"/>.
/// Handles loading secrets only once and caches the connection string in-memory.
/// </summary>
public class DbConnectionFactory(ISecretProvider secretProvider, IOptions<AwsSettings> awsOptions, IConfiguration configuration) : IDbConnectionFactory
{
    private readonly AwsSettings _awsSettings = awsOptions.Value;

    // Local cache of the generated connection string to avoid repeated lookups.
    private string? _cachedConnectionString;

    public async Task<string> GetConnectionStringAsync()
    {
        if (_cachedConnectionString != null)
            return _cachedConnectionString;

        // 1) Try to get connection string from configuration (useful for local dev)
        var localConnectionString = configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrEmpty(localConnectionString))
        {
            _cachedConnectionString = localConnectionString;
            return _cachedConnectionString;
        }

        // 2) Otherwise, load from AWS Secrets Manager
        var secretId = _awsSettings.Secrets?.DatabaseSecretId
                       ?? throw new InvalidOperationException("AWS:Secrets:DatabaseSecretId not configured.");

        var secretJson = await secretProvider.GetSecretAsync(secretId)
                         ?? throw new InvalidOperationException($"Secret '{secretId}' not found in AWS Secrets Manager.");

        // Deserialize JSON into our DbCredentials object.
        var creds = JsonSerializer.Deserialize<DbCredentials>(secretJson, JsonDefaults.CaseInsensitive)
                    ?? throw new InvalidOperationException($"Failed to parse credentials from secret '{secretId}'.");

        _cachedConnectionString = creds.ToConnectionString();
        return _cachedConnectionString;
    }
}
