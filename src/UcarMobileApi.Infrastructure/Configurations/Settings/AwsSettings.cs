using System.ComponentModel.DataAnnotations;

namespace UcarMobileApi.Infrastructure.Configurations.Settings;

public class AwsSettings
{
    [Required]
    public string Region { get; set; } = null!;

    [Required]
    public CognitoSettings Cognito { get; set; } = null!;

    public AwsSesOptions Ses { get; set; } = new();
    public AwsSnsOptions Sns { get; set; } = new();
}

public class CognitoSettings
{
    [Required]
    public string UserPoolId { get; set; } = null!;

    [Required]
    public string ClientId { get; set; } = null!;

    public int ClockSkewMinutes { get; set; } = 2;
}


public class AwsSesOptions
{
    public string FromEmail { get; set; } = null!;
}

public class AwsSnsOptions
{
    public string SenderId { get; set; } = null!;
}