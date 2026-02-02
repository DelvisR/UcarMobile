using System;

namespace UcarMobileApi.Application.Common.Helpers;

public static class PaymentHelper
{
    public static bool IsCardExpired(int expMonth, int expYear)
    {
        if (expMonth < 1 || expMonth > 12)
            return true;

        // Último instante del mes de expiración (UTC)
        var expirationDateUtc = new DateTime(
            expYear,
            expMonth,
            DateTime.DaysInMonth(expYear, expMonth),
            23, 59, 59,
            DateTimeKind.Utc
        );

        return expirationDateUtc < DateTime.UtcNow;
    }

}
