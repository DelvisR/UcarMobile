namespace UcarMobileApi.Core.Constants;

public static class BusinessParameterKeys
{
    public static class Company
    {
        public const string Name = "COMPANY_NAME";
        public const string Address = "COMPANY_HEADQUARTERS_ADDRESS";
        public const string Phone = "COMPANY_CONTACT_PHONE";
        public const string Email = "COMPANY_CONTACT_EMAIL";
        public const string Web = "COMPANY_WEBSITE_URL";
    }

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

    public static class Warranty
    {
        public const string WarrantyMonths = "WARRANTY_MONTHS";
        public const string WarrantyMiles = "WARRANTY_MILES";
    }

    public static class Discount
    {
        public const string ReviewDiscountAmount = "REVIEW_DISCOUNT_AMOUNT";
    }

    public static class Appointment
    {
        public const string AdjustmentWindowHours = "APPOINTMENT_ADJUSTMENT_WINDOW_HOURS";
        public const string AdjustmentFeeAmount = "APPOINTMENT_ADJUSTMENT_FEE_AMOUNT";
        public const string MinServiceFeeAmount = "APPOINTMENT_MIN_SERVICE_FEE_AMOUNT";
    }
}
