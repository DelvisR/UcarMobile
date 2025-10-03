namespace UcarMobileApi.Core.Entities.Notifications;

/// <summary>
/// Represents the payload for a push notification.
/// </summary>
public class PushMessage
{
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
