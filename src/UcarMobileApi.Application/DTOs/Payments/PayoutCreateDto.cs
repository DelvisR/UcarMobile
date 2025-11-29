namespace UcarMobileApi.Application.DTOs.Payments;

/// <summary>
/// Request model to create or refresh the onboarding link.
/// </summary>
public class OnboardRequestDto
{
    /// <summary>
    /// URL where the technician will be redirected after successful onboarding.
    /// </summary>
    public string ReturnUrl { get; set; } = string.Empty;

    /// <summary>
    /// URL where the technician will be redirected if the link expires or onboarding must be restarted.
    /// </summary>
    public string RefreshUrl { get; set; } = string.Empty;
}

/// <summary>
/// Response containing the onboarding URL for the technician.
/// </summary>
public class OnboardResponseDto
{
    /// <summary>
    /// Onboarding URL to be opened by the technician in the browser.
    /// </summary>
    public string OnboardingUrl { get; set; } = string.Empty;
}

/// <summary>
/// DTO used to request a payout to a technician.
/// </summary>
public record PayoutCreateDto(string IdempotencyKey, long AmountCents, string Currency = "usd");

/// <summary>
/// Represents the result of attempting to transfer a payout to a technician.
/// </summary>
public class PayoutDto
{
    public string Status { get; set; } = "";
    public long AmountCents { get; set; }
    public string Currency { get; set; } = "";
    public string? ErrorCode { get; set; }
}



