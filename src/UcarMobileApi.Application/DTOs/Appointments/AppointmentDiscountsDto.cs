using UcarMobileApi.Core.Entities.Appointments;

namespace UcarMobileApi.Application.DTOs.Appointments;

public class ApplyDiscountDto
{
    public DiscountCategory Category { get; set; }
    public DiscountType Type { get; set; }
    public decimal Value { get; set; }
    public string Reason { get; set; } = null!;
    public string? Code { get; set; }
    public DiscountSource Source { get; set; }
}

public class AppointmentDiscountDto
{
    public DiscountCategory Category { get; set; }
    public DiscountType Type { get; set; }
    public DiscountSource Source { get; set; }
    public decimal Amount { get; set; }
    public string? Code { get; set; }
    public string Reason { get; set; } = null!;
}

