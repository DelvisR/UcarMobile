// Presentation/Controllers/NotificationsController.cs

using Microsoft.AspNetCore.Mvc;
using UcarMobileApi.Authorization;
using UcarMobileApi.Core.Entities.Notifications;
using UcarMobileApi.Infrastructure.Services.Notifications;

namespace UcarMobileApi.Controllers.Notifications;

/// <summary>
/// API controller that exposes endpoints for creating notifications.
/// Instead of sending immediately, notifications are enqueued to SQS
/// for asynchronous processing by the BackgroundService.
/// </summary>
[ApiController]
[Route("api/notifications")]
public class NotificationsController(NotificationQueuePublisher publisher, ILogger<NotificationsController> logger) : ControllerBase
{
    /// <summary>
    /// Enqueue an email notification for asynchronous delivery.
    /// Requires 'ACTION_SEND_NOTIFICATION' action.
    /// <response code="202">Returns Accepted.</response>
    /// </summary>
    [HttpPost("email")]
    [RequireAction("ACTION_SEND_NOTIFICATION")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> SendEmail([FromBody] EmailMessage request, CancellationToken ct)
    {
        var message = new NotificationMessage
        {
            Type = NotificationType.Email,
            Email = request
        };

        await publisher.PublishAsync(message, ct);

        logger.LogInformation("Email notification enqueued for {To}", request.To);

        return Accepted(new { status = "enqueued", type = "email", to = request.To });
    }

    /// <summary>
    /// Enqueue an SMS notification for asynchronous delivery.
    /// Requires 'ACTION_SEND_NOTIFICATION' action.
    /// <response code="202">Returns Accepted.</response>
    /// </summary>
    [HttpPost("sms")]
    [RequireAction("ACTION_SEND_NOTIFICATION")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> SendSms([FromBody] SmsMessage request, CancellationToken ct)
    {
        var message = new NotificationMessage
        {
            Type = NotificationType.Sms,
            Sms = request
        };

        await publisher.PublishAsync(message, ct);

        logger.LogInformation("SMS notification enqueued for {To}", request.To);

        return Accepted(new { status = "enqueued", type = "sms", to = request.To });
    }

    /// <summary>
    /// Enqueue a push notification for asynchronous delivery.
    /// Requires 'ACTION_SEND_NOTIFICATION' action.
    /// <response code="202">Returns Accepted.</response>
    /// </summary>
    [HttpPost("push")]
    [RequireAction("ACTION_SEND_NOTIFICATION")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> SendPush([FromBody] PushMessage request, CancellationToken ct)
    {
        var message = new NotificationMessage
        {
            Type = NotificationType.Push,
            Push = request
        };

        await publisher.PublishAsync(message, ct);

        logger.LogInformation("Push notification enqueued for {UserId}", request.UserId);

        return Accepted(new { status = "enqueued", type = "push", userId = request.UserId });
    }
}
