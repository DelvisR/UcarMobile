using System;
using System.Collections.Generic;
using System.Linq;
using UcarMobileApi.Core.Entities.Common;
using UcarMobileApi.Core.Exceptions;

namespace UcarMobileApi.Core.Entities.Appointments;

public partial class Appointment
{
    private readonly List<AppointmentDiscount> _discounts = [];

    public IReadOnlyCollection<AppointmentDiscount> Discounts => _discounts.AsReadOnly();

    public void ApplyDiscount(AppointmentDiscount discount)
    {
        if (_discounts.Any(d => d.Category == discount.Category))
            throw new BusinessException($"Discount {discount.Category} already applied.");

        _discounts.Add(discount);
        EstimatedTotal = Math.Max(0, EstimatedTotal - discount.Amount);
    }

    public AppointmentDiscount? RemoveDiscount(DiscountCategory category)
    {
        var discount = _discounts.SingleOrDefault(d => d.Category == category);
        if (discount == null) return null;

        _discounts.Remove(discount);
        EstimatedTotal += discount.Amount;
        return discount;
    }
}

public class AppointmentDiscount : EntityBase
{
    public int AppointmentId { get; private set; }
    public Appointment Appointment { get; private set; } = null!;

    public DiscountCategory Category { get; private set; }
    public DiscountType Type { get; private set; }

    public decimal Value { get; private set; }
    public decimal Amount { get; private set; }

    public string? Code { get; private set; }
    public string Reason { get; private set; } = null!;
    public DiscountSource Source { get; private set; }

    private AppointmentDiscount() { }

    public AppointmentDiscount(
        int appointmentId,
        DiscountCategory category,
        DiscountType type,
        decimal value,
        decimal baseAmount,
        string reason,
        DiscountSource source,
        string? code = null)
    {
        if (value <= 0)
            throw new BusinessException("Discount value must be greater than zero.");

        AppointmentId = appointmentId;
        Category = category;
        Type = type;
        Value = value;
        Code = code;
        Reason = reason;
        Source = source;

        Amount = Calculate(baseAmount);
    }

    private decimal Calculate(decimal baseAmount)
    {
        return Type switch
        {
            DiscountType.Flat => Value,
            DiscountType.Percentage => Math.Round(baseAmount * (Value / 100m), 2),
            _ => 0
        };
    }
}

public enum DiscountCategory : byte
{
    Review = 1,
    Coupon = 2
}

public enum DiscountType : byte
{
    Flat = 1,
    Percentage = 2
}

public enum DiscountSource : byte
{
    Client = 1,
    Technician = 2,
    Agent = 3,
    Admin = 4
}
