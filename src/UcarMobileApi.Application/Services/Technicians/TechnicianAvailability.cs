using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Core.Constants;
using UcarMobileApi.Core.Entities.Technicians;

namespace UcarMobileApi.Application.Services.Technicians;

public static class TechnicianAvailability
{
    /// <summary>
    /// Generates a cross-technician availability map for the next N days.
    /// </summary>
    /// <param name="db">Context</param>
    /// <param name="ct"></param>
    /// <param name="businessParameters"></param>
    /// <param name="includeToday">Whether the computation should include today's date</param>
    /// <returns>A dictionary keyed by date with a list of available slots ("T08:00:00-T08:30:00")</returns>
    /// <remarks>The result is calculated by combining the availability of all technicians.</remarks>
    public static async Task<Dictionary<string, List<string>>> GetAvailableSlotsAsync(IAppDbContext db, CancellationToken ct,
        BusinessParameterService businessParameters, bool includeToday = true)
    {
        var days = await businessParameters.GetValueAsync<int>(BusinessParameterKeys.Scheduling.BookingCalendarDaysAhead, ct); // 30 days
        var slotMinutes = await businessParameters.GetValueAsync<int>(BusinessParameterKeys.Scheduling.DiagnosticWindowMinutes, ct); //60 min

        var today = DateTime.UtcNow.Date;
        var start = includeToday ? today : today.AddDays(1);
        var end = start.AddDays(days);

        // -------------------------------------------
        // 1) Load technicians and all necessary data
        // -------------------------------------------
        var technicians = await db.Set<Technician>()
            .Where(t => t.IsActive)
            .Include(t => t.WorkSchedules.Where(ws => ws.IsActive))
            .Include(t => t.CalendarBlocks.Where(cb => cb.IsActive))
            // Only appointments from today forward
            .Include(t => t.Appointments.Where(av => av.Appointment.ScheduledStart >= today))
            .ThenInclude(av => av.Appointment)
            // A technician without WorkSchedules is automatically ignored
            .Where(t => t.WorkSchedules.Any(ws => ws.IsActive))
            .ToListAsync(ct);

        var result = new Dictionary<string, List<string>>();

        // ------------------------------------------------------
        // 2) Iterate day-by-day and combine availability
        // ------------------------------------------------------
        foreach (var date in EachDay(start, end))
        {
            var dailySlots = new HashSet<string>();

            foreach (var tech in technicians)
            {
                // 2.1. Load base schedule ranges
                var scheduleRanges = tech.WorkSchedules
                    .Where(ws => ws.Day == date.DayOfWeek)
                    .Select(ws => (ws.StartTime, ws.EndTime))
                    .ToList();

                if (scheduleRanges.Count == 0)
                    continue;

                // 2.2. Apply blocks
                var blocks = tech.CalendarBlocks
                    .Where(cb =>
                        (cb.SpecificDate.HasValue && cb.SpecificDate.Value.Date == date.Date)
                        ||
                        (cb.WeeklyDay.HasValue && cb.WeeklyDay.Value == date.DayOfWeek))
                    .ToList();

                scheduleRanges = ApplyBlocks(scheduleRanges, blocks);

                if (scheduleRanges.Count == 0)
                    continue;

                // 2.3. Apply appointments (default 4h duration)
                var appointments = tech.Appointments
                    .Where(a => a.Appointment.ScheduledStart.Date == date.Date)
                    .Select(a =>
                    {
                        var startTime = a.Appointment.ScheduledStart.TimeOfDay;
                        var endTime = (a.Appointment.ScheduledEnd ??
                                       a.Appointment.ScheduledStart.AddHours(4))
                            .TimeOfDay;
                        return (startTime, endTime);
                    })
                    .ToList();

                scheduleRanges = ApplyAppointments(scheduleRanges, appointments);

                // 2.4. Split into slot intervals
                var slots = SplitIntoSlots(scheduleRanges, slotMinutes);
                foreach (var slot in slots)
                    dailySlots.Add(slot);
            }

            if (dailySlots.Count != 0)
            {
                result[date.ToString("yyyy-MM-dd")] =
                    [.. dailySlots.OrderBy(x => x)];
            }
        }

        return result;
    }

    // =====================================================================
    // ------------------------- INTERNAL HELPERS ---------------------------
    // =====================================================================

    /// <summary>
    /// Generates each day from start to end inclusive.
    /// </summary>
    private static IEnumerable<DateTime> EachDay(DateTime from, DateTime to)
    {
        for (var day = from; day <= to; day = day.AddDays(1))
            yield return day;
    }

    /// <summary>
    /// Applies blocks to a list of allowed time ranges.
    /// </summary>
    private static List<(TimeSpan Start, TimeSpan End)> ApplyBlocks(List<(TimeSpan Start, TimeSpan End)> ranges, List<TechnicalCalendarBlock> blocks)
    {
        foreach (var block in blocks)
        {
            if (block.AllDay)
            {
                ranges.Clear();
                return ranges;
            }

            if (block.StartTime is null || block.EndTime is null)
                continue;

            var updated = new List<(TimeSpan Start, TimeSpan End)>();

            foreach (var range in ranges)
            {
                if (block.EndTime <= range.Start || block.StartTime >= range.End)
                {
                    updated.Add(range);
                }
                else
                {
                    if (range.Start < block.StartTime)
                        updated.Add((range.Start, block.StartTime.Value));

                    if (range.End > block.EndTime)
                        updated.Add((block.EndTime.Value, range.End));
                }
            }

            ranges = updated;
        }

        return ranges;
    }

    /// <summary>
    /// Removes time ranges occupied by appointments.
    /// </summary>
    private static List<(TimeSpan Start, TimeSpan End)> ApplyAppointments(List<(TimeSpan Start, TimeSpan End)> ranges,
        List<(TimeSpan Start, TimeSpan End)> appointments)
    {
        foreach (var (start, end) in appointments)
        {
            var updated = new List<(TimeSpan Start, TimeSpan End)>();

            foreach (var range in ranges)
            {
                if (end <= range.Start || start >= range.End)
                {
                    updated.Add(range);
                }
                else
                {
                    if (range.Start < start)
                        updated.Add((range.Start, start));

                    if (range.End > end)
                        updated.Add((end, range.End));
                }
            }

            ranges = updated;
        }

        return ranges;
    }

    /// <summary>
    /// Converts schedule ranges into fixed-duration time slots.
    /// </summary>
    private static List<string> SplitIntoSlots(List<(TimeSpan Start, TimeSpan End)> ranges, int minutes)
    {
        var output = new List<string>();

        foreach (var (start, end) in ranges)
        {
            var cursor = start;

            while (cursor + TimeSpan.FromMinutes(minutes) <= end)
            {
                var next = cursor + TimeSpan.FromMinutes(minutes);
                output.Add($@"T{cursor:hh\:mm\:ss}-T{next:hh\:mm\:ss}");
                cursor = next;
            }
        }

        return output;
    }
}
