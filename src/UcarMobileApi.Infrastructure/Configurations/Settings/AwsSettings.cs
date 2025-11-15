using System;
using System.ComponentModel.DataAnnotations;

namespace UcarMobileApi.Infrastructure.Configurations.Settings;

/// <summary>
/// Root AWS settings loaded from configuration.
/// Includes region, Secrets Manager options, Cognito, SES, SNS, etc.
/// </summary>
public class AwsSettings
{
    [Required]
    public string Region { get; set; } = null!;

    [Required]
    public string AccountId { get; set; } = null!;

    /// <summary>Settings for AWS Secrets Manager.</summary>
    public AwsSecretsOptions Secrets { get; set; } = new();

    [Required]
    public CognitoSettings Cognito { get; set; } = null!;

    public AwsSesOptions Ses { get; set; } = new();
    public AwsSnsOptions Sns { get; set; } = new();
    public AwsPushOptions Push { get; set; } = new();
    public AwsSqsOptions Sqs { get; set; } = new();
    public AwsS3Options S3 { get; set; } = new();
}

/// <summary>
/// Settings for AWS Secrets Manager.
/// </summary>
public class AwsSecretsOptions
{
    /// <summary>
    /// Default cache TTL for Secrets Manager cache in minutes. Defaults to 15 minutes.
    /// </summary>
    public int CacheMinutes { get; set; } = 15;

    /// <summary>Name or ARN of the database secret.</summary>
    public string? DatabaseSecretId { get; set; }

    /// <summary>Name or ARN of the Cognito root secret.</summary>
    public string? CognitoRootSecretId { get; set; }

    /// <summary>Name or ARN of the Stripe secret.</summary>
    public string? StripeSecretId { get; set; }

    /// <summary>Name or ARN of the Google API key secret.</summary>
    public string? GoogleApiKeySecretId { get; set; }
}

/// <summary>Cognito settings (User Pool, Client ID, etc.).</summary>
public class CognitoSettings
{
    [Required]
    public string UserPoolId { get; set; } = null!;
    [Required]
    public string ClientId { get; set; } = null!;
    public int ClockSkewMinutes { get; set; } = 2;
}

/// <summary>SES settings.</summary>
public class AwsSesOptions
{
    [Required]
    public string FromEmail { get; set; } = null!;
}

/// <summary>SNS settings.</summary>
public class AwsSnsOptions
{
    public string SenderId { get; set; } = "App"; // Default
    public string DefaultSmsType { get; set; } = "Transactional";
}

/// <summary>Push notification settings.</summary>
public class AwsPushOptions
{
    public string AndroidPlatformArn { get; set; } = string.Empty;
    public string IosPlatformArn { get; set; } = string.Empty;

    // Computed property to build AndroidPlatformArn (Android)
    public string GetAndroidPlatformArn(string region, string accountId)
    {
        if (string.IsNullOrWhiteSpace(region) ||
            string.IsNullOrWhiteSpace(accountId) ||
            string.IsNullOrWhiteSpace(AndroidPlatformArn))
        {
            throw new InvalidOperationException("Missing AWS SQS configuration");
        }

        return $"arn:aws:sns:{region}:{accountId}:app/{AndroidPlatformArn}";
    }

    // Computed property to build IosPlatformArn (iOS)
    public string GetIosPlatformArn(string region, string accountId)
    {
        if (string.IsNullOrWhiteSpace(region) ||
            string.IsNullOrWhiteSpace(accountId) ||
            string.IsNullOrWhiteSpace(AndroidPlatformArn))
        {
            throw new InvalidOperationException("Missing AWS SQS configuration");
        }

        return $"arn:aws:sns:{region}:{accountId}:app/{IosPlatformArn}";
    }
}

/// <summary>Sqs settings.</summary>
public class AwsSqsOptions
{
    public string QueueName { get; set; } = string.Empty;

    // Computed property to build QueueUrl
    public string GetQueueUrl(string region, string accountId)
    {
        if (string.IsNullOrWhiteSpace(region) ||
            string.IsNullOrWhiteSpace(accountId) ||
            string.IsNullOrWhiteSpace(QueueName))
        {
            throw new InvalidOperationException("Missing AWS SQS configuration");
        }

        return $"https://sqs.{region}.amazonaws.com/{accountId}/{QueueName}";
    }
}

public class AwsS3Options
{
    /// <summary>The S3 bucket name that stores files.</summary>
    public string BucketName { get; set; } = string.Empty;

    /// <summary>Optional base path/prefix inside bucket (e.g., "app-data").</summary>
    public string BasePath { get; set; } = "app-data";

    /// <summary>Default presigned URL expiration in minutes.</summary>
    public int PresignMinutes { get; set; } = 10;

    /// <summary>Use path-style URLs if needed (usually false).</summary>
    public bool UsePathStyle { get; set; } = false;
}
