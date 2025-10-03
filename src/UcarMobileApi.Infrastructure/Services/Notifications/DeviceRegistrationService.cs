using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using UcarMobileApi.Application.DTOs.Users;
using UcarMobileApi.Application.Validators.Users;
using UcarMobileApi.Core.Entities.Users;
using UcarMobileApi.Core.Enums;
using UcarMobileApi.Infrastructure.Configurations.Settings;
using UcarMobileApi.Infrastructure.Data;

namespace UcarMobileApi.Infrastructure.Services.Notifications;

/// <summary>
/// Service responsible for registering devices with AWS SNS for push notifications.
/// </summary>
/// <param name="sns">Injected AWS SNS client.</param>
/// <param name="db">Injected database context.</param>
/// <param name="settings">Injected AWS configuration settings.</param>
public class DeviceRegistrationService(IAmazonSimpleNotificationService sns, IServiceScopeFactory scopeFactory, IOptions<AwsSettings> settings)
{
    private readonly AwsSettings _settings = settings.Value;

    /// <summary>
    /// Registers a device with AWS SNS and stores the endpoint ARN in the database.
    /// </summary>
    /// <param name="dto">RegisterDevice dto.</param>
    /// <param name="ct">Cancellation token for request cancellation.</param>
    /// <returns>Returns the SNS endpoint ARN associated with the registered device.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the platform is unsupported.</exception>
    public async Task<string> RegisterDeviceAsync(RegisterDeviceDto dto, CancellationToken ct = default)
    {
        var validator = new DeviceValidator();
        await validator.ValidateAndThrowAsync(dto, ct);

        var platformArn = dto.Platform switch
        {
            DevicePlatform.Android => _settings.Push.GetAndroidPlatformArn(_settings.Region, _settings.AccountId),
            DevicePlatform.iOS => _settings.Push.GetIosPlatformArn(_settings.Region, _settings.AccountId),
            _ => throw new InvalidOperationException("Unsupported platform")
        };

        var response = await sns.CreatePlatformEndpointAsync(new CreatePlatformEndpointRequest
        {
            PlatformApplicationArn = platformArn,
            Token = dto.Token,
            CustomUserData = dto.UserId.ToString()
        }, ct);

        var device = new Device
        {
            UserId = dto.UserId,
            Token = dto.Token,
            Platform = dto.Platform,
            EndpointArn = response.EndpointArn
        };

        using var scope = scopeFactory.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.Set<Device>().Add(device);

        await db.SaveChangesAsync(ct);

        return response.EndpointArn;
    }
}
