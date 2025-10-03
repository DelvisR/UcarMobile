namespace UcarMobileApi.Infrastructure.Data;

/// <summary>
/// Represents database credentials stored in AWS Secrets Manager.
/// Includes safe defaults when fields are missing.
/// </summary>
public class DbCredentials
{
    /// <summary>
    /// Maps to "host" in AWS Secrets Manager.
    /// Default = "localhost" if missing.
    /// </summary>
    public string Host { get; set; } = "localhost";

    /// <summary>
    /// Maps to "port" in AWS Secrets Manager.
    /// Default = 5432 if missing.
    /// </summary>
    public int Port { get; set; } = 5432;

    /// <summary>
    /// Maps to "dbInstanceIdentifier" (or can be set manually in AWS).
    /// Default = "postgres" if missing.
    /// </summary>
    public string Database { get; set; } = "ucarmobile";

    /// <summary>
    /// Maps to "username" in AWS Secrets Manager.
    /// Default = "postgres" if missing.
    /// </summary>
    public string Username { get; set; } = "postgres";

    /// <summary>
    /// Maps to "password" in AWS Secrets Manager.
    /// Default = "postgres" if missing.
    /// </summary>
    public string Password { get; set; } = "postgres";

    /// <summary>
    /// Builds the PostgreSQL connection string including pooling.
    /// Uses defensive defaults in case any field is empty or null.
    /// </summary>
    public string ToConnectionString()
    {
        // Defensive assignment in case of empty strings
        var host = string.IsNullOrWhiteSpace(Host) ? "localhost" : Host;
        var port = Port == 0 ? 5432 : Port;
        var database = string.IsNullOrWhiteSpace(Database) ? "postgres" : Database;
        var user = string.IsNullOrWhiteSpace(Username) ? "postgres" : Username;
        var pwd = string.IsNullOrWhiteSpace(Password) ? "postgres" : Password;

        return $"Host={host};Port={port};Database={database};Username={user};Password={pwd};" +
               "Pooling=true;Minimum Pool Size=5;Maximum Pool Size=50;SslMode=Require;";
    }
}
