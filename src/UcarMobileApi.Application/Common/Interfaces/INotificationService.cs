using System.Threading;
using System.Threading.Tasks;
using UcarMobileApi.Core.Enums;

namespace UcarMobileApi.Application.Common.Interfaces;

/// <summary>
/// Unified notification service contract.
/// Consumers should use this instead of injecting Email, SMS, or Push services individually.
/// </summary>
public interface INotificationService
{
    Task SendEmailAsync(string to, string subject, string body, CancellationToken ct = default);
    Task SendTemplateEmailAsync(string to, EmailTemplate templateName, string templateDataJson, CancellationToken ct = default);
    Task SendSmsAsync(string to, string message, CancellationToken ct = default);
    Task SendPushAsync(int userId, string title, string message, CancellationToken ct = default);
}
