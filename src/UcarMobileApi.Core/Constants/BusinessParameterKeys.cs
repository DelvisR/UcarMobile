namespace UcarMobileApi.Core.Constants;

public static class BusinessParameterKeys
{
    public const string DefaultClientRoles = "DEFAULT_CLIENT_ROLES";
    public static class Scheduling
    {
        public const string DiagnosticWindowMinutes = "DIAGNOSTIC_WINDOW_MINUTES";
        public const string RepairBufferHours = "REPAIR_BUFFER_HOURS";
        public const string BookingCalendarDaysAhead = "BOOKING_CALENDAR_DAYS_AHEAD";
    }

    public static class Estimate
    {
        public const string LaborDiscountFactor = "LABOR_DISCOUNT_FACTOR";
        public const string PartDiscountFactor = "PART_DISCOUNT_FACTOR";
    }
}
