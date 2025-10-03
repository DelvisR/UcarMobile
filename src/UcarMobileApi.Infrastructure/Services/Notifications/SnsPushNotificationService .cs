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

public class SnsPushNotificationService(IAmazonSimpleNotificationService sns, IServiceScopeFactory scopeFactory,
    ILogger<SnsPushNotificationService> logger) : IPushNotificationService
{
    public async Task SendPushAsync(int userId, string title, string body, CancellationToken ct = default)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var devices = await db.Set<Device>()
            .AsNoTracking()
            .Where(d => d.UserId == userId && !d.IsDeleted)
            .ToListAsync(ct);

        if (devices.Count == 0) return;

        var gcmPayload = JsonSerializer.Serialize(new { notification = new { title, body } });
        var apnsPayload = JsonSerializer.Serialize(new { aps = new { alert = new { title, body } } });

        var publishRequests = devices.Select(device => new PublishRequest
        {
            TargetArn = device.EndpointArn,
            MessageStructure = "json",
            Message = JsonSerializer.Serialize(new Dictionary<string, object>
            {
                ["default"] = body,
                ["GCM"] = gcmPayload,
                ["APNS"] = apnsPayload
            })

        }).ToList();

        var tasks = publishRequests.Select(async request =>
        {
            try
            {
                await sns.PublishAsync(request, ct);
            }
            catch (Exception ex)
            {
                logger.LogError("Error enviando a {RequestTargetArn}: {ExMessage}", request.TargetArn, ex.Message);
            }
        });

        await Task.WhenAll(tasks);
    }
}
