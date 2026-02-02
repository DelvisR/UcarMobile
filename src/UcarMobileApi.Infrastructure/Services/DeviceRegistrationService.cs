using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using UcarMobileApi.Application.DTOs.Users;
using UcarMobileApi.Application.Validators.Users;
using UcarMobileApi.Core.Entities.Users;
using UcarMobileApi.Core.Enums;
using UcarMobileApi.Infrastructure.Configurations.Settings;
using UcarMobileApi.Infrastructure.Data;

namespace UcarMobileApi.Infrastructure.Services;

/// <summary>
/// Service responsible for registering devices with AWS SNS for push notifications.
/// </summary>
/// <param name="sns">Injected AWS SNS client.</param>
/// <param name="scopeFactory">Database scope factory.</param>
/// <param name="settings">Injected AWS configuration settings.</param>
public class DeviceRegistrationService(IAmazonSimpleNotificationService sns, IServiceScopeFactory scopeFactory, IOptions<AwsSettings> settings)
{
    private readonly AwsSettings _settings = settings.Value;

    /// <summary>
    /// Registers a device with AWS SNS and stores the endpoint ARN in the database.
    /// </summary>
    /// <param name="authProviderId">The Cognito ID of the user.</param>
    /// <param name="dto">RegisterDevice dto.</param>
    /// <param name="ct">Cancellation token for request cancellation.</param>
    /// <returns>Returns the SNS endpoint ARN associated with the registered device.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the platform is unsupported.</exception>
    public async Task<string> RegisterDeviceAsync(string authProviderId, RegisterDeviceDto dto, CancellationToken ct = default)
    {
        await new DeviceValidator().ValidateAndThrowAsync(dto, ct);

        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Resolve UserId from Cognito sub
        var userId = await db.Set<User>()
            .AsNoTracking()
            .Where(u => u.AuthProviderId == authProviderId)
            .Select(u => u.Id)
            .FirstOrDefaultAsync(ct);

        if (userId == 0)
            throw new NotFoundException($"User with AuthProviderId '{authProviderId}' not found.");

        // Search for existing device (IDEMPOTENCE)
        var existingDevice = await db.Set<Device>()
            .FirstOrDefaultAsync(d => d.UserId == userId && d.Token == dto.Token && d.Platform == dto.Platform, ct);

        if (existingDevice is not null)
        {
            await EnsureEndpointEnabledAsync(existingDevice.EndpointArn, dto.Token, ct);

            return existingDevice.EndpointArn;
        }

        // Create SNS endpoint only if it does NOT exist
        var platformArn = dto.Platform switch
        {
            DevicePlatform.Android => _settings.Push.GetAndroidPlatformArn(_settings.Region, _settings.AccountId),
            DevicePlatform.iOS => _settings.Push.GetIosPlatformArn(_settings.Region, _settings.AccountId),
            _ => throw new InvalidOperationException("Unsupported platform")
        };

        var response = await sns.CreatePlatformEndpointAsync(
            new CreatePlatformEndpointRequest
            {
                PlatformApplicationArn = platformArn,
                Token = dto.Token,
                CustomUserData = userId.ToString()
            },
            ct);

        var device = new Device
        {
            UserId = userId,
            Token = dto.Token,
            Platform = dto.Platform,
            EndpointArn = response.EndpointArn
        };

        db.Set<Device>().Add(device);
        await db.SaveChangesAsync(ct);

        return response.EndpointArn;
    }

    private async Task EnsureEndpointEnabledAsync(string endpointArn, string token, CancellationToken ct)
    {
        var attributesResponse = await sns.GetEndpointAttributesAsync(new GetEndpointAttributesRequest { EndpointArn = endpointArn }, ct);

        var attributes = attributesResponse.Attributes;

        var needsUpdate = attributes.TryGetValue("Enabled", out var enabled) && enabled != "true" ||
                          attributes.TryGetValue("Token", out var existingToken) && existingToken != token;

        if (!needsUpdate)
            return;

        await sns.SetEndpointAttributesAsync(
            new SetEndpointAttributesRequest
            {
                EndpointArn = endpointArn,
                Attributes = new Dictionary<string, string>
                {
                    ["Enabled"] = "true",
                    ["Token"] = token
                }
            },
            ct);
    }

}
