using System;

namespace UcarMobileApi.Application.DTOs.Technicians;

/// <summary>DTO used to create or update a weekly recurring work schedule.</summary>
public class TechnicalWorkScheduleDto
{
    /// <summary>Technician id.</summary>
    public int TechnicianId { get; set; }
    /// <summary>Day of week (0=Sunday..6=Saturday).</summary>
    public DayOfWeek Day { get; set; }
    /// <summary>Start time of the schedule.</summary>
    public TimeSpan StartTime { get; set; }
    /// <summary>End time of the schedule.</summary>
    public TimeSpan EndTime { get; set; }
    /// <summary>Is the schedule active?</summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>Read DTO for TechnicalWorkSchedule.</summary>
public class TechnicalWorkScheduleReadDto : TechnicalWorkScheduleDto
{
    /// <summary>Schedule id.</summary>
    public int Id { get; set; }
}

/// <summary>DTO used to create/update a calendar block (affectation).</summary>
public class TechnicalCalendarBlockDto
{
    /// <summary>Technician id.</summary>
    public int TechnicianId { get; set; }
    /// <summary>Optional specific date (date-only).</summary>
    public DateOnly? SpecificDate { get; set; }
    /// <summary>Optional weekly day (recurring weekly block).</summary>
    public DayOfWeek? WeeklyDay { get; set; }
    /// <summary>Start time for partial-day block.</summary>
    public TimeSpan? StartTime { get; set; }
    /// <summary>End time for partial-day block.</summary>
    public TimeSpan? EndTime { get; set; }
    /// <summary>Whether this block covers the whole day.</summary>
    public bool AllDay { get; set; } = false;
    /// <summary>Is the block active?</summary>
    public bool IsActive { get; set; } = true;
    /// <summary>Reason for the block (vacation, training, etc.).</summary>
    public string Reason { get; set; } = string.Empty;
}

public class TechnicalCalendarBlockReadDto : TechnicalCalendarBlockDto
{
    public int Id { get; set; }
}
