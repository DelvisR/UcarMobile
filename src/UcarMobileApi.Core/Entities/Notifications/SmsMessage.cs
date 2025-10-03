namespace UcarMobileApi.Core.Entities.Notifications;

/// <summary>
/// Represents the payload for an SMS notification.
/// </summary>
public class SmsMessage
{
    public string To { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
