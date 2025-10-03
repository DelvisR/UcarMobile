using System.Threading;
using System.Threading.Tasks;
using UcarMobileApi.Application.Common.Interfaces;

namespace UcarMobileApi.Infrastructure.Services.Notifications;

/// <summary>
/// Unified notification service that encapsulates Email, SMS, and Push notification services.
/// This service provides a single entry point to send different types of notifications.
/// </summary>
public class NotificationService(IEmailService emailService, ISmsService smsService, IPushNotificationService pushService) : INotificationService
{
    /// <summary>
    /// Send an email notification.
    /// </summary>
    public Task SendEmailAsync(string to, string subject, string body, CancellationToken ct = default)
        => emailService.SendEmailAsync(to, subject, body, ct);

    /// <summary>
    /// Send an SMS notification.
    /// </summary>
    public Task SendSmsAsync(string to, string message, CancellationToken ct = default)
        => smsService.SendSmsAsync(to, message, ct);

    /// <summary>
    /// Send a push notification.
    /// </summary>
    public Task SendPushAsync(int userId, string title, string message, CancellationToken ct = default)
        => pushService.SendPushAsync(userId, title, message, ct);
}
