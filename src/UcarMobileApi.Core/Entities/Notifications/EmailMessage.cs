using UcarMobileApi.Core.Enums;

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

// string to, EmailTemplate template, T model
public class TemplateEmailMessage
{
    public string To { get; set; } = string.Empty;
    public EmailTemplate Template { get; set; }
    public string TemplateDataJson { get; init; } = string.Empty;
}
