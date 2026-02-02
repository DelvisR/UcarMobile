using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UcarMobileApi.Application.Common.Interfaces;

namespace UcarMobileApi.Infrastructure.Services.Storage;

public class StoredFileCleanupBackgroundService(IServiceProvider serviceProvider, ILogger<StoredFileCleanupBackgroundService> logger) : BackgroundService
{
    // Configuration for the cleanup interval
    private readonly TimeSpan _cleanupInterval = TimeSpan.FromDays(10);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("StoredFile Cleanup Background Service started.");

        // Execute immediately on startup (optional)
        await RunCleanupSafely(stoppingToken);

        // Use PeriodicTimer for accurate scheduling and better cancellation handling
        using var timer = new PeriodicTimer(_cleanupInterval);

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await RunCleanupSafely(stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Graceful shutdown requested
            logger.LogInformation("StoredFile Cleanup Background Service is stopping.");
        }
    }

    private async Task RunCleanupSafely(CancellationToken ct)
    {
        try
        {
            logger.LogInformation("Starting scheduled cleanup task...");

            using var scope = serviceProvider.CreateScope();
            var cleanup = scope.ServiceProvider.GetRequiredService<IStoredFileCleanupService>();

            await cleanup.RunCleanupAsync(ct);

            logger.LogInformation("Scheduled cleanup task finished.");
        }
        catch (Exception ex)
        {
            // Catch-all to ensure the PeriodicTimer loop does not crash
            logger.LogError(ex, "StoredFile cleanup CRITICAL failure during execution.");
        }
    }
}
