using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace UcarMobileApi.Infrastructure.Data
{
    /// <summary>
    /// Applies pending EF Core migrations at application startup.
    /// </summary>
    public static class DatabaseInitializer
    {
        /// <summary>
        /// Ensures the database exists and applies pending migrations asynchronously.
        /// </summary>
        /// <param name="serviceProvider">The application's service provider.</param>
        /// <returns>A <see cref="Task"/> representing the async operation.</returns>
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            // Create a logger for this initializer without needing a generic type
            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("DatabaseInitializer");

            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            try
            {
                logger.LogInformation("Checking database existence and applying migrations...");

                // Creates DB if missing, applies all pending migrations
                await dbContext.Database.MigrateAsync();

                logger.LogInformation("Database is up to date.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while migrating or initializing the database.");
                // throw; // Optional: stop app startup if migrations fail
            }
        }
    }
}