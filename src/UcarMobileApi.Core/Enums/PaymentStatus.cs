namespace UcarMobileApi.Core.Enums;

public enum PaymentStatus : byte
{
    Unpaid = 1,
    Partial,
    Paid,
    Refunded,
    Failed
}
