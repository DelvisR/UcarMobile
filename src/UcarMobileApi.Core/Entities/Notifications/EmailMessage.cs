namespace UcarMobileApi.Core.Entities.Notifications;

/// <summary>
/// Represents the payload for an email notification.
/// </summary>
public class EmailMessage
{
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}
