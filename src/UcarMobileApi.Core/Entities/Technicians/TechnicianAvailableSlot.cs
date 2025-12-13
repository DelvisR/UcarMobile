using System;

namespace UcarMobileApi.Core.Entities.Technicians;

public sealed class TechnicianAvailableSlot
{
    public DateTime SlotDate { get; set; }
    public TimeSpan SlotStart { get; set; }
    public TimeSpan SlotEnd { get; set; }
}
