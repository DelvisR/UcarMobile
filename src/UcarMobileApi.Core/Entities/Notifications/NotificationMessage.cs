using System.Text.Json.Serialization;

namespace UcarMobileApi.Core.Entities.Notifications;

/// <summary>
/// Wrapper for different notification types.
/// </summary>
public class NotificationMessage
{
    public NotificationType Type { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public EmailMessage? Email { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public TemplateEmailMessage? TemplateEmail { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public SmsMessage? Sms { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public PushMessage? Push { get; set; }
}
