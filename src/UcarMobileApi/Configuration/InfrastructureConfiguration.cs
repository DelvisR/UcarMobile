using Amazon;
using Amazon.SimpleEmailV2;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Infrastructure.Configurations.Settings;
using UcarMobileApi.Infrastructure.Services.Notifications;

namespace UcarMobileApi.Configuration;

/// <summary>
/// Provides extension methods to register Infrastructure-layer services.
/// Encapsulates AWS clients and notification services.
/// </summary>
public static class InfrastructureConfiguration
{
    /// <summary>
    /// Registers Infrastructure services, including AWS SDK clients and notification services.
    /// </summary>
    /// <param name="services">The service collection to register services in.</param>
    /// <param name="awsSettings">The AWS settings containing region configuration.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, AwsSettings awsSettings)
    {
        // Determine effective AWS region, prioritizing AWS_REGION environment variable
        var region = RegionEndpoint.GetBySystemName(awsSettings.Region);

        // Register AWS SDK clients as singletons
        // IAmazonSimpleEmailServiceV2 for SES (Simple Email Service)
        services.AddSingleton<IAmazonSimpleEmailServiceV2>(_ => new AmazonSimpleEmailServiceV2Client(region));

        // IAmazonSimpleNotificationService for SNS (SMS and Push)
        services.AddSingleton<IAmazonSimpleNotificationService>(_ => new AmazonSimpleNotificationServiceClient(region));

        // Register notification services that implement the Application-layer interfaces
        services.AddSingleton<IEmailService, SesEmailService>();           // SES email service
        services.AddSingleton<ISmsService, SnsSmsService>();               // SNS SMS service
        services.AddSingleton<DeviceRegistrationService>();                // Device Registration
        services.AddSingleton<IPushNotificationService, SnsPushNotificationService>(); // SNS push notifications

        // Unified NotificationService (facade)
        services.AddSingleton<INotificationService, NotificationService>();

        // register the Amazon SQS client
        services.AddAWSService<IAmazonSQS>();

        // register your publisher and background worker
        services.AddSingleton<NotificationQueuePublisher>(sp =>
        {
            var sqs = sp.GetRequiredService<IAmazonSQS>();
            return new NotificationQueuePublisher(sqs, awsSettings);
        });

        // Register your BackgroundService in the .NET dependency container so that it runs automatically in parallel
        // when the API is launched.
        services.AddHostedService<NotificationBackgroundService>();

        return services;
    }
}
