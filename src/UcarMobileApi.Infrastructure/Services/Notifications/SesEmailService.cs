using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SimpleEmailV2;
using Amazon.SimpleEmailV2.Model;
using Microsoft.Extensions.Options;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Core.Enums;
using UcarMobileApi.Infrastructure.Configurations.Settings;

namespace UcarMobileApi.Infrastructure.Services.Notifications;

public static class EmailTemplateExtensions
{
    public static string ToSesName(this EmailTemplate template) => template switch
    {
        EmailTemplate.WelcomeUser => "SEMPLATES_DEMO",
        EmailTemplate.ResetPassword => "ResetPassword",
        EmailTemplate.OtpCode => "OtpCode",
        _ => throw new ArgumentOutOfRangeException(nameof(template))
    };
}

/// <summary>
/// AWS SES email service implementation.
/// Uses strongly typed <see cref="AwsSettings"/> instead of direct IConfiguration.
/// </summary>
public class SesEmailService(IAmazonSimpleEmailServiceV2 ses, IOptions<AwsSettings> options) : IEmailService
{
    private readonly string _fromAddress = options.Value.Ses.FromEmail
                                           ?? throw new InvalidOperationException("AWS SES FromEmail not configured");

    public async Task SendEmailAsync(string to, string subject, string body, CancellationToken ct = default)
    {
        var request = new SendEmailRequest
        {
            FromEmailAddress = _fromAddress,
            Destination = new Destination
            {
                ToAddresses = [to]
            },
            Content = new EmailContent
            {
                Simple = new Message
                {
                    Subject = new Content { Data = subject },
                    Body = new Body
                    {
                        Html = new Content { Data = body }
                    }
                }
            }
        };

        var result = await ses.SendEmailAsync(request, ct);
    }

    public async Task SendTemplateEmailAsync(string to, EmailTemplate template, string templateDataJson, CancellationToken ct = default)
    {
        var request = new SendEmailRequest
        {
            FromEmailAddress = _fromAddress,
            Destination = new Destination
            {
                ToAddresses = [to]
            },
            Content = new EmailContent
            {
                Template = new Template
                {
                    TemplateName = template.ToSesName(),
                    TemplateData = templateDataJson
                }
            }
        };

        await ses.SendEmailAsync(request, ct);
    }
}
