using Microsoft.AspNetCore.Mvc;
using UcarMobileApi.Application.DTOs.Users;
using UcarMobileApi.Infrastructure.Services.Notifications;

namespace UcarMobileApi.Controllers.Users;

/// <summary>
/// Controller responsible for device registration for push notifications.
/// </summary>
[ApiController]
[Route("api/devices")]
public class DevicesController(DeviceRegistrationService registration) : ControllerBase
{
    /// <summary>
    /// Registers a device for push notifications using the provided token and platform.
    /// </summary>
    /// <param name="dto">The device registration data including user ID, token, and platform.</param>
    /// <param name="ct">Cancellation token for request cancellation.</param>
    /// <returns>Returns the ARN (Amazon Resource Name) of the registered device endpoint.</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDeviceDto dto, CancellationToken ct)
    {
        var arn = await registration.RegisterDeviceAsync(dto, ct);
        return Ok(new { EndpointArn = arn });
    }
}
