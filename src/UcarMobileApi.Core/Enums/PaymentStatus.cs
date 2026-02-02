namespace UcarMobileApi.Core.Enums;

public enum PaymentStatus : byte
{
    Unpaid = 1,
    Paid,
    Refunded,
    Failed
}
