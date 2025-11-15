using System;
using System.Net.Http;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sentry;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Core.Entities.Notifications;
using UcarMobileApi.Infrastructure.Configurations.Settings;

namespace UcarMobileApi.Infrastructure.Services.Notifications;

/// <summary>
/// Background worker that consumes messages from SQS and dispatches them
/// to the appropriate notification service (email, SMS, push).
/// </summary>
public class NotificationBackgroundService(IAmazonSQS sqs, INotificationService notificationService, IOptions<AwsSettings> options,
    ILogger<NotificationBackgroundService> logger) : BackgroundService
{

    private readonly AwsSettings _awsSettings = options.Value;

    /// <summary>
    /// Main loop: long-poll ReceiveMessage, dispatch, delete on success.
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queueUrl = _awsSettings.Sqs.GetQueueUrl(_awsSettings.Region, _awsSettings.AccountId);

        while (!stoppingToken.IsCancellationRequested)
        {
            // Long polling reduces empty responses and cost
            var receiveRequest = new ReceiveMessageRequest
            {
                QueueUrl = queueUrl,
                MaxNumberOfMessages = 10,
                WaitTimeSeconds = 20, // long polling
                // Request system attributes
                MessageSystemAttributeNames = ["All"],

                // Custom attributes you define at SendMessage
                MessageAttributeNames = ["All"]
            };

            try
            {
                var response = await sqs.ReceiveMessageAsync(receiveRequest, stoppingToken);

                foreach (var msg in response.Messages ?? [])
                {
                    try
                    {
                        var notification = JsonSerializer.Deserialize<NotificationMessage>(msg.Body);
                        if (notification == null)
                        {
                            logger.LogWarning("Invalid message JSON: {Body}", msg.Body);

                            if (SentrySdk.IsEnabled)
                                SentrySdk.CaptureMessage($"Invalid SQS message JSON: {msg.Body}");

                            // delete the malformed message to avoid infinite retries, or move to DLQ manually
                            await sqs.DeleteMessageAsync(queueUrl, msg.ReceiptHandle, stoppingToken);
                            continue;
                        }

                        // Dispatch to the correct channel using your facade
                        await DispatchNotificationAsync(notification, stoppingToken);

                        // Delete from queue only if processed successfully
                        await sqs.DeleteMessageAsync(queueUrl, msg.ReceiptHandle, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Failed to process SQS message {MessageId}", msg.MessageId);

                        if (SentrySdk.IsEnabled)
                            SentrySdk.CaptureException(ex);

                        await sqs.DeleteMessageAsync(queueUrl, msg.ReceiptHandle, stoppingToken);

                        // OR Do NOT delete the message; SQS will make it visible again and
                        // after maxReceiveCount it will move to the DLQ if exist.
                        //
                        // If the processing may take longer than VisibilityTimeout,
                        // consider calling ChangeMessageVisibilityAsync to extend it.
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                await HandlePollingErrorAsync(ex, "Network error, will retry...", stoppingToken);
            }
            catch (SocketException ex)
            {
                await HandlePollingErrorAsync(ex, "Socket/DNS error, will retry...", stoppingToken);
            }
            catch (Exception ex)
            {
                await HandlePollingErrorAsync(ex, "Unexpected error.", stoppingToken);
            }
        }
    }

    private async Task HandlePollingErrorAsync(Exception ex, string context, CancellationToken ct)
    {
        logger.LogError(ex, "Error while polling SQS. Context: {Context}", context);

        if (SentrySdk.IsEnabled)
            SentrySdk.CaptureException(ex);

        // Simple configurable optional echo delay
        await Task.Delay(TimeSpan.FromSeconds(10), ct);
    }

    private async Task DispatchNotificationAsync(NotificationMessage message, CancellationToken ct)
    {
        switch (message.Type)
        {
            case NotificationType.Email when message.Email is not null:
                await notificationService.SendEmailAsync(message.Email.To, message.Email.Subject, message.Email.Body, ct);
                break;

            case NotificationType.Sms when message.Sms is not null:
                await notificationService.SendSmsAsync(message.Sms.To, message.Sms.Message, ct);
                break;

            case NotificationType.Push when message.Push is not null:
                await notificationService.SendPushAsync(message.Push.UserId, message.Push.Title, message.Push.Message, ct);
                break;

            default:
                logger.LogWarning("Unsupported notification type or missing payload: {Type}", message.Type);
                break;
        }
    }
}
