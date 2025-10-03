using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SimpleEmailV2;
using Amazon.SimpleEmailV2.Model;
using Microsoft.Extensions.Options;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Infrastructure.Configurations.Settings;

namespace UcarMobileApi.Infrastructure.Services.Notifications;

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

        await ses.SendEmailAsync(request, ct);
    }
}
