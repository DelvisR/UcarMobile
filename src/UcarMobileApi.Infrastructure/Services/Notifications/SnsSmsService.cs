using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Options;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Infrastructure.Configurations.Settings;

namespace UcarMobileApi.Infrastructure.Services.Notifications;

public class SnsSmsService(IAmazonSimpleNotificationService sns, IOptions<AwsSettings> options) : ISmsService
{
    private readonly AwsSettings _settings = options.Value;

    public async Task SendSmsAsync(string phoneNumber, string message, CancellationToken ct = default)
    {

        var request = new PublishRequest
        {
            Message = message,
            PhoneNumber = phoneNumber,
            MessageAttributes = new Dictionary<string, MessageAttributeValue>
            {
                ["AWS.SNS.SMS.SMSType"] = new MessageAttributeValue
                {
                    StringValue = _settings.Sns.DefaultSmsType,
                    DataType = "String"
                },
                ["AWS.SNS.SMS.SenderID"] = new MessageAttributeValue
                {
                    StringValue = _settings.Sns.SenderId,
                    DataType = "String"
                }
            }
        };

        await sns.PublishAsync(request, ct);
    }
}
