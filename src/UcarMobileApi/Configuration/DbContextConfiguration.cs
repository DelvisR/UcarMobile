using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using UcarMobileApi.Infrastructure.Data;
using UcarMobileApi.Infrastructure.Services;

namespace UcarMobileApi.Configuration;

/// <summary>
/// Provides extension methods to configure the application's database context.
/// Supports flexible configuration for both Development and Production:
/// - If "DefaultConnection" is provided, it will be used directly.
/// - Otherwise, "SecretId" must be configured and credentials will be retrieved 
///   from AWS Secrets Manager via ISecretProvider (with internal caching).
/// Secrets are cached internally by SecretsManagerCache; no additional local cache is required.
/// </summary>
public static class DbContextConfiguration
{
    /// <summary>
    /// Registers and configures the application's database context.
    /// </summary>
    public static IServiceCollection AddAppDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        // Register DbContext
        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            // Try to get connection string directly from configuration
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // If not provided, resolve from AWS Secrets Manager
            if (string.IsNullOrEmpty(connectionString))
            {
                var secretId = configuration.GetConnectionString("SecretId")
                    ?? throw new InvalidOperationException("No DefaultConnection or SecretId configured.");

                var secretProvider = serviceProvider.GetRequiredService<ISecretProvider>();

                // Get secret JSON synchronously
                var secretJson = secretProvider.GetSecretAsync(secretId).GetAwaiter().GetResult()
                                 ?? throw new InvalidOperationException($"Secret '{secretId}' not found in AWS Secrets Manager.");

                // Deserialize secret into database credentials
                var dbCredentials = JsonSerializer.Deserialize<DbCredentials>(secretJson)
                    ?? throw new InvalidOperationException($"Failed to parse database credentials from secret '{secretId}'.");

                // Construct PostgreSQL connection string dynamically
                connectionString = $"Host={dbCredentials.Host};Port={dbCredentials.Port};Database={dbCredentials.Database};" +
                                   $"Username={dbCredentials.Username};Password={dbCredentials.Password}";
            }

            // Configure Npgsql with NetTopologySuite for PostGIS support
            options.UseNpgsql(connectionString, o => o.UseNetTopologySuite());
        });

        return services;
    }

    /// <summary>
    /// Represents database credentials stored in AWS Secrets Manager.
    /// The secret JSON must match this structure:
    /// {
    ///   "Host": "hostname",
    ///   "Port": 5432,
    ///   "Database": "dbname",
    ///   "Username": "dbuser",
    ///   "Password": "dbpass"
    /// }
    /// </summary>
    internal class DbCredentials
    {
        public string Host { get; set; } = null!;
        public int Port { get; set; }
        public string Database { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
