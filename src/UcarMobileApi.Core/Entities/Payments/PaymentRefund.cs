using UcarMobileApi.Core.Entities.Common;

namespace UcarMobileApi.Core.Entities.Payments;

public class PaymentRefund : EntityBase
{
    public int PaymentId { get; set; }
    public string ProviderRefundId { get; set; } = string.Empty;
    public long AmountCents { get; set; }
    public string Currency { get; set; } = "usd";
    public string Status { get; set; } = "pending";

    public Payment Payment { get; set; } = null!;
}
