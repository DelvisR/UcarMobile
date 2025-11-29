using System;

namespace UcarMobileApi.Core.Entities.Technicians;

public class TechnicalWorkSchedule : EntityBase
{
    public int TechnicianId { get; set; }
    public Technician Technician { get; set; } = null!;
    public DayOfWeek Day { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsActive { get; set; } = true;
}

public class TechnicalCalendarBlock : EntityBase
{
    public int TechnicianId { get; set; }
    public Technician Technician { get; set; } = null!;
    public DateTime? SpecificDate { get; set; }
    public DayOfWeek? WeeklyDay { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public bool AllDay { get; set; } = false;
    public string Reason { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
