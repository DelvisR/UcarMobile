using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using UcarMobileApi.Infrastructure.Data;

namespace UcarMobileApi.Infrastructure.Factories;

/// <summary>
/// Design-time factory for creating <see cref="AppDbContext"/> instances.
/// This is required by Entity Framework Core tools (e.g., migrations)
/// to create the DbContext without running Program.cs.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // Build configuration manually since Program.cs won't be executed at design-time.
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true) // Use environment-specific settings if available
            .AddJsonFile("appsettings.Development.local.json", optional: true) // local
            .AddEnvironmentVariables()
            .Build();

        // Retrieve the connection string from configuration.
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException(
                "No connection string found for design-time DbContext creation. " +
                "Please configure a valid connection string in appsettings.json or environment variables."
            );

        // Configure DbContext options for PostgreSQL + NetTopologySuite
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString, o => o.UseNetTopologySuite());

        // Return a new AppDbContext instance with default HttpContextAccessor
        return new AppDbContext(optionsBuilder.Options, new HttpContextAccessor());
    }
}
