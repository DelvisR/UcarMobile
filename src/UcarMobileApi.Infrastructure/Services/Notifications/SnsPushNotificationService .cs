using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Core.Entities.Users;
using UcarMobileApi.Infrastructure.Data;

namespace UcarMobileApi.Infrastructure.Services.Notifications;

/// <summary>
/// Handles push notifications using AWS SNS. 
/// Safe for Singleton registration — creates its own scoped DbContext per operation.
/// </summary>
public class SnsPushNotificationService(
    IAmazonSimpleNotificationService sns,
    IServiceScopeFactory scopeFactory,
    ILogger<SnsPushNotificationService> logger
) : IPushNotificationService
{
    /// <summary>
    /// Sends a push notification to all devices of a given user.
    /// Returns a list of invalid or disabled endpoint ARNs.
    /// </summary>
    public async Task<List<string>> SendPushAsync(int userId, string title, string body, CancellationToken ct = default)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var devices = await db.Set<Device>()
            .AsNoTracking()
            .Where(d => d.UserId == userId && !d.IsDeleted)
            .ToListAsync(ct);

        if (devices.Count == 0)
            return [];

        var gcmPayload = JsonSerializer.Serialize(new { notification = new { title, body } });
        var apnsPayload = JsonSerializer.Serialize(new { aps = new { alert = new { title, body } } });

        var invalidEndpoints = new List<string>();

        var tasks = devices.Select(async device =>
        {
            var request = new PublishRequest
            {
                TargetArn = device.EndpointArn,
                MessageStructure = "json",
                Message = JsonSerializer.Serialize(new Dictionary<string, object>
                {
                    ["default"] = body,
                    ["GCM"] = gcmPayload,
                    ["APNS"] = apnsPayload
                })
            };

            try
            {
                await sns.PublishAsync(request, ct);
            }
            catch (AmazonSimpleNotificationServiceException ex)
            {
                logger.LogWarning("SNS error for {Arn}: {Code} - {Msg}", request.TargetArn, ex.ErrorCode, ex.Message);

                if (ex.ErrorCode is "EndpointDisabled" or "InvalidParameter")
                {
                    lock (invalidEndpoints)
                        invalidEndpoints.Add(request.TargetArn);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error sending to {Arn}", request.TargetArn);
            }
        });

        await Task.WhenAll(tasks);

        return invalidEndpoints;
    }

    /// <summary>
    /// Removes invalid SNS endpoints both from SNS and from the database.
    /// Creates its own service scope to ensure DbContext lifetime safety.
    /// </summary>
    public async Task CleanInvalidEndpointsAsync(int userId, IEnumerable<string> endpointArns, CancellationToken ct = default)
    {
        if (!endpointArns.Any())
            return;

        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var devices = db.Set<Device>();

        // Filter by userId + endpointArn
        var invalidDevices = await devices
            .Where(d => d.UserId == userId && endpointArns.Contains(d.EndpointArn))
            .ToListAsync(ct);

        foreach (var device in invalidDevices)
            await TryRemoveDeviceAsync(db, device, ct);

        await db.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Attempts to remove a device from both SNS and the database.
    /// </summary>
    private async Task TryRemoveDeviceAsync(AppDbContext db, Device device, CancellationToken ct)
    {
        try
        {
            await sns.DeleteEndpointAsync(new DeleteEndpointRequest
            {
                EndpointArn = device.EndpointArn
            }, ct);
        }
        catch (NotFoundException)
        {
            // Already deleted in SNS
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting endpoint {Arn} from SNS", device.EndpointArn);
            return; // Skip DB deletion if SNS failed unexpectedly
        }

        db.Remove(device);
        logger.LogInformation("Endpoint removed from SNS and DB for user {UserId}: {Arn}", device.UserId, device.EndpointArn);
    }
}
