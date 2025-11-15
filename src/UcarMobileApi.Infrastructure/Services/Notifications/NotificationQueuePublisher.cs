// Infrastructure/Services/Notifications/NotificationQueuePublisher.cs

using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using UcarMobileApi.Core.Entities.Notifications;
using UcarMobileApi.Infrastructure.Configurations.Settings;

namespace UcarMobileApi.Infrastructure.Services.Notifications;

/// <summary>
/// Publishes notification messages to an AWS SQS queue.
/// </summary>
public class NotificationQueuePublisher(IAmazonSQS sqs, AwsSettings awsSettings)
{
    public async Task PublishAsync(NotificationMessage message, CancellationToken ct = default)
    {
        var queueUrl = awsSettings.Sqs.GetQueueUrl(awsSettings.Region, awsSettings.AccountId);

        var body = JsonSerializer.Serialize(message);
        var request = new SendMessageRequest
        {
            QueueUrl = queueUrl,
            MessageBody = body
        };
        var result = await sqs.SendMessageAsync(request, ct);
        Console.WriteLine(result.MessageId);
    }
}
