using Amazon;
using Amazon.S3;
using Amazon.SimpleEmailV2;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Infrastructure.Configurations.Settings;
using UcarMobileApi.Infrastructure.Providers;
using UcarMobileApi.Infrastructure.Reports;
using UcarMobileApi.Infrastructure.Services;
using UcarMobileApi.Infrastructure.Services.Location;
using UcarMobileApi.Infrastructure.Services.Notifications;
using UcarMobileApi.Infrastructure.Services.Payments;
using UcarMobileApi.Infrastructure.Services.Storage;

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
        // Register the IHttpContextAccessor service in the dependency container.
        services.AddHttpContextAccessor();

        #region AWS Notification

        // Determine effective AWS region
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

        // Register the Amazon S3 client
        services.AddSingleton<IAmazonS3>(_ =>
        {
            var cfg = new AmazonS3Config
            {
                RegionEndpoint = region,
                ForcePathStyle = awsSettings.S3.UsePathStyle
            };
            return new AmazonS3Client(cfg);
        });

        // register your publisher and background worker
        services.AddSingleton<NotificationQueuePublisher>(sp =>
        {
            var sqs = sp.GetRequiredService<IAmazonSQS>();
            return new NotificationQueuePublisher(sqs, awsSettings);
        });

        // Register your BackgroundService in the .NET dependency container so that it runs automatically in parallel
        // when the API is launched. Then, is Singleton by default
        services.AddHostedService<NotificationBackgroundService>();

        #endregion

        #region Stripe

        // Register Stripe Payment Processing service and webhook
        services.AddScoped<IPaymentService, StripePaymentService>();
        services.AddScoped<IPaymentWebHookService, StripePaymentWebHookService>();

        #endregion

        #region Google API

        // Register Google API key factory as singleton for caching
        services.AddSingleton<GoogleApiKeyProvider>();

        // Register HTTP client for LocationService
        services.AddHttpClient<ILocationService, LocationService>();

        // Register location service as scoped
        services.AddScoped<ILocationService, LocationService>();

        #endregion

        #region Storage

        // Register File Storage service (S3 implementation)
        services.AddScoped<IStorageService, S3StorageService>();

        #endregion

        #region Report

        services.AddScoped<IReportService, FastReportService>();

        #endregion

        return services;
    }
}
