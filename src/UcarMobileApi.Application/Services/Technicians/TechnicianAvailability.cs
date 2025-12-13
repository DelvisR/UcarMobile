using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using PublicHoliday;
using UcarMobileApi.Application.Common.Helpers;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Application.DTOs.Technicians;
using UcarMobileApi.Core.Constants;
using UcarMobileApi.Core.Entities.Services;
using UcarMobileApi.Core.Entities.Technicians;

namespace UcarMobileApi.Application.Services.Technicians;

public static class TechnicianAvailability
{
    /// <summary>
    /// Returns the nearest available technician that can cover the service at the given UTC time range.
    /// All input dates must be in UTC. Uses real spherical distance (meters) via PostGIS geography.
    /// Handles partial blocks with time overlap and ignores past specific-date blocks.
    /// </summary>
    /// <param name="db">Database context</param>
    /// <param name="businessParameters">Global business parameters service</param>
    /// <param name="lat">Client latitude</param>
    /// <param name="lng">Client longitude</param>
    /// <param name="zipCode">Client ZIP code</param>
    /// <param name="specialties">Required service category IDs</param>
    /// <param name="appointmentStartUtc">Appointment start time in UTC</param>
    /// <param name="appointmentEndUtc">Appointment end time in UTC (typically start + 4h)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The closest available technician, or null if none found</returns>
    public static async Task<Technician?> GetNearestAvailableTechnicianAsync(IAppDbContext db,
        BusinessParameterService businessParameters, double lat, double lng, string zipCode, List<int> specialties,
        DateTime appointmentStartUtc,   // Already in UTC
        DateTime appointmentEndUtc,     // Already in UTC
        CancellationToken ct = default)
    {
        var apptDuration = await businessParameters.GetValueAsync<int>(BusinessParameterKeys.Scheduling.RepairBufferHours, ct); //4 h

        // 1. Create client point (SRID 4326)
        var clientPoint = new Point(lng, lat) { SRID = 4326 };

        // 2. Get active service zones that cover this ZIP code
        var serviceZoneIds = await db.Set<ServiceZone>()
            .Where(sz => sz.IsActive && sz.ZipCodes.Contains(zipCode))
            .Select(sz => sz.Id)
            .ToListAsync(ct);

        if (serviceZoneIds.Count == 0)
            return null;

        // Extract day of week and date for filtering (from UTC, but DayOfWeek is invariant)
        var appointmentDayOfWeek = appointmentStartUtc.DayOfWeek;
        var appointmentDateOnly = DateOnly.FromDateTime(appointmentStartUtc);
        var appointmentStartTime = appointmentStartUtc.TimeOfDay;
        var appointmentEndTime = appointmentEndUtc.TimeOfDay;

        // 3. Main query: find the closest technician with full availability and correct filters
        return await db.Set<Technician>()
            .AsNoTracking()
            .Where(t => t.IsActive

                // Covers a service zone that includes the client's ZIP
                && t.ServiceZones.Any(sz => serviceZoneIds.Contains(sz.ServiceZoneId))

                // Has all required specialties
                && specialties.All(requiredId => t.Specialities.Any(s => s.ServiceCategoryId == requiredId))

                // Has an active work schedule for this day of week
                && t.WorkSchedules.Any(ws => ws.IsActive && ws.Day == appointmentDayOfWeek)

                // No overlapping block: full-day or partial time range
                // - For SpecificDate: only if date >= appointment date (ignore past)
                // - For WeeklyDay: always if day matches
                // - Overlap: block start/end intersects with appointment start/end
                && !t.CalendarBlocks.Any(cb => cb.IsActive
                    && (
                        // Specific date block (future or current only)
                        (cb.SpecificDate.HasValue
                         && cb.SpecificDate.Value >= appointmentDateOnly
                         && cb.SpecificDate.Value == appointmentDateOnly
                         && (
                             cb.AllDay
                             || (cb.StartTime.HasValue && cb.EndTime.HasValue
                                 && cb.StartTime.Value < appointmentEndTime
                                 && cb.EndTime.Value > appointmentStartTime)
                         ))
                        ||
                        // Weekly recurring block
                        (cb.WeeklyDay.HasValue
                         && cb.WeeklyDay.Value == appointmentDayOfWeek
                         && (
                             cb.AllDay
                             || (cb.StartTime.HasValue && cb.EndTime.HasValue
                                 && cb.StartTime.Value < appointmentEndTime
                                 && cb.EndTime.Value > appointmentStartTime)
                         ))
                    ))

                // No overlapping appointment (handles null ScheduledEnd safely)
                && !t.Appointments.Any(a =>
                    a.Appointment.ScheduledStart < appointmentEndUtc &&
                    (a.Appointment.ScheduledEnd ?? a.Appointment.ScheduledStart.AddHours(apptDuration)) > appointmentStartUtc)
            )
            // Real distance using PostGIS geography (spheroid)
            .OrderBy(t => EF.Functions.Distance(t.BaseAddress.BasePoint, clientPoint, false))
            //.ProjectTo<TechnicianDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }

    #region usin sql
    /// <summary>
    /// Generates a cross-technician availability map for the next N days (SQL Raw).
    /// </summary>
    /// <param name="db">Context</param>
    /// <param name="request">AvailableSlotRequestDto</param>
    /// <param name="ct">Cancellation Token</param>
    /// <param name="businessParameters"></param>
    /// <returns>A dictionary keyed by date with a list of available slots ("T08:00:00-T08:30:00")</returns>
    /// <remarks>The result is calculated by combining the availability of all technicians, excluding US public holidays.</remarks>
    public static async Task<Dictionary<string, List<string>>> GetAvailableSlotsSqlAsync(IAppDbContext db, BusinessParameterService businessParameters,
    AvailableSlotRequestDto request, CancellationToken ct)
    {
        // -------------------------------------------------------
        // 1. Business parameters
        // -------------------------------------------------------
        var days = await businessParameters.GetValueAsync<int>(BusinessParameterKeys.Scheduling.BookingCalendarDaysAhead, ct);

        var slotMinutes = await businessParameters.GetValueAsync<int>(BusinessParameterKeys.Scheduling.DiagnosticWindowMinutes, ct);

        var apptDurationHours = await businessParameters.GetValueAsync<int>(BusinessParameterKeys.Scheduling.RepairBufferHours, ct);

        // -------------------------------------------------------
        // 2. Timezone + dates
        // -------------------------------------------------------
        var tzInfo = TimeZoneHelper.GetTimeZoneInfo(request.Lat, request.Lng);
        var usaHolidays = new USAPublicHoliday();

        var todayLocal = DateTime.UtcNow.ConvertUtcToLocalTime(tzInfo).Date;

        var startLocal = request.IncludeToday ? todayLocal : todayLocal.AddDays(1);

        var endLocal = startLocal.AddDays(days);

        var startUtc = startLocal.ConvertLocalTimeToUtc(tzInfo);
        var endUtc = endLocal.AddDays(1).AddSeconds(-1).ConvertLocalTimeToUtc(tzInfo);

        var currentLocalTime = DateTime.UtcNow.ConvertUtcToLocalTime(tzInfo);

        // -------------------------------------------------------
        // 3. Holidays array (DATE[])
        // -------------------------------------------------------
        var holidaySet = PrecomputeHolidays(usaHolidays, startLocal, endLocal);

        var holidayArray = holidaySet.Select(d => d.Date).ToArray();

        // -------------------------------------------------------
        // 4. Specialties parameter (INT[])
        // -------------------------------------------------------
        var specialtyArray = request.Specialties.ToArray();

        // -------------------------------------------------------
        // 5. SQL
        // -------------------------------------------------------
        const string sql = """

                   WITH dates AS (
                       SELECT generate_series(
                           @startLocal::date,
                           @endLocal::date,
                           interval '1 day'
                       )::date AS work_date
                   ),
                   slots AS (
                       SELECT 
                           d.work_date,
                           t."Id" AS technician_id,
                           ws."StartTime" AS schedule_start,
                           ws."EndTime" AS schedule_end
                       FROM dates d

                       JOIN "Technician" t
                           ON TRUE

                       JOIN "UserAccount" u
                           ON u."Id" = t."Id"
                          AND u."IsActive" = TRUE

                       JOIN "TechnicalWorkSchedule" ws
                           ON ws."TechnicianId" = t."Id"
                          AND ws."IsActive" = TRUE
                          AND ws."Day" = EXTRACT(DOW FROM d.work_date)

                       JOIN "TechnicianServiceZone" szt
                           ON szt."TechnicianId" = t."Id"

                       JOIN "ServiceZone" sz
                           ON sz."Id" = szt."ServiceZoneId"
                          AND sz."IsActive" = TRUE
                          AND @zipCode = ANY(sz."ZipCodes")

                       WHERE NOT (d.work_date = ANY(@holidays))
                         AND (
                               array_length(@specialties, 1) IS NULL 
                               OR t."Id" IN (
                                   SELECT s."TechnicianId"
                                   FROM "TechnicianSpeciality" s
                                   WHERE s."ServiceCategoryId" = ANY(@specialties)
                                   GROUP BY s."TechnicianId"
                                   HAVING COUNT(DISTINCT s."ServiceCategoryId") = array_length(@specialties, 1)
                               )
                             )
                   ),
                   slot_generation AS (
                       SELECT 
                           s.work_date,
                           generate_series(
                               s.work_date + s.schedule_start,
                               s.work_date + (s.schedule_end - (@slotMinutes || ' minutes')::interval),
                               (@slotMinutes || ' minutes')::interval
                           ) AS slot_start,
                           generate_series(
                               s.work_date + s.schedule_start,
                               s.work_date + (s.schedule_end - (@slotMinutes || ' minutes')::interval),
                               (@slotMinutes || ' minutes')::interval
                           ) + (@slotMinutes || ' minutes')::interval AS slot_end,
                           s.technician_id
                       FROM slots s
                   ),
                   filtered AS (
                       SELECT DISTINCT
                           g.work_date AS "SlotDate",
                           g.slot_start::time AS "SlotStart",
                           g.slot_end::time AS "SlotEnd"
                       FROM slot_generation g

                       WHERE NOT EXISTS (
                           SELECT 1
                           FROM "AppointmentVehicle" av
                           JOIN "Appointment" a ON a."Id" = av."AppointmentId"
                           WHERE av."TechnicianId" = g.technician_id
                             AND a."ScheduledStart" < g.slot_end
                             AND COALESCE(a."ScheduledEnd", a."ScheduledStart" + (@apptDuration || ' hours')::interval) > g.slot_start
                       )

                       AND NOT EXISTS (
                           SELECT 1
                           FROM "TechnicalCalendarBlock" b
                           WHERE b."TechnicianId" = g.technician_id
                           AND b."IsActive" = TRUE
                           AND (
                               (b."SpecificDate" = g.work_date)
                               OR
                               (b."WeeklyDay" = EXTRACT(DOW FROM g.work_date))
                           )
                           AND (
                               b."AllDay" = TRUE
                               OR
                               (
                                   g.slot_start::time < b."EndTime"
                                   AND g.slot_end::time > b."StartTime"
                               )
                           )
                       )
                   )

                   SELECT
                       "SlotDate",
                       "SlotStart",
                       "SlotEnd"
                   FROM filtered
                   ORDER BY "SlotDate", "SlotStart";

                   """;

        // -------------------------------------------------------
        // 6. Execute SQL
        // -------------------------------------------------------
        var rows = await db.Set<TechnicianAvailableSlot>()
            .FromSqlRaw(sql,
                new Npgsql.NpgsqlParameter("@startLocal", startLocal),
                new Npgsql.NpgsqlParameter("@endLocal", endLocal),
                new Npgsql.NpgsqlParameter("@zipCode", request.ZipCode),
                new Npgsql.NpgsqlParameter("@slotMinutes", slotMinutes),
                new Npgsql.NpgsqlParameter("@apptDuration", apptDurationHours),
                new Npgsql.NpgsqlParameter("@specialties", specialtyArray) { NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Integer },
                new Npgsql.NpgsqlParameter("@holidays", holidayArray) { NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Date })
            .AsNoTracking()
            .ToListAsync(ct);

        // -------------------------------------------------------
        // 7. Filter slots past time for today + format
        // -------------------------------------------------------
        var result = rows
            .Where(r => r.SlotDate > todayLocal || r.SlotStart > currentLocalTime.TimeOfDay)
            .GroupBy(r => r.SlotDate.ToString("yyyy-MM-dd"))
            .ToDictionary(
                g => g.Key,
                g => g.OrderBy(x => x.SlotStart).Select(x => $@"T{x.SlotStart:hh\:mm\:ss}-T{x.SlotEnd:hh\:mm\:ss}").Distinct().ToList()
            );

        return result;
    }


    #endregion

    /// <summary>
    /// Generates a cross-technician availability map for the next N days.
    /// </summary>
    /// <param name="db">Context</param>
    /// <param name="request">AvailableSlotRequestDto</param>
    /// <param name="ct">Cancellation Token</param>
    /// <param name="businessParameters"></param>
    /// <returns>A dictionary keyed by date with a list of available slots ("T08:00:00-T08:30:00")</returns>
    /// <remarks>The result is calculated by combining the availability of all technicians, excluding US public holidays.</remarks>
    public static async Task<Dictionary<string, List<string>>> GetAvailableSlotsAsync(IAppDbContext db, BusinessParameterService businessParameters,
        AvailableSlotRequestDto request, CancellationToken ct)
    {
        // Fetch business parameters
        var days = await businessParameters.GetValueAsync<int>(BusinessParameterKeys.Scheduling.BookingCalendarDaysAhead, ct);
        var slotMinutes = await businessParameters.GetValueAsync<int>(BusinessParameterKeys.Scheduling.DiagnosticWindowMinutes, ct);
        var apptDuration = await businessParameters.GetValueAsync<int>(BusinessParameterKeys.Scheduling.RepairBufferHours, ct);

        var tzInfo = TimeZoneHelper.GetTimeZoneInfo(request.Lat, request.Lng);
        var usaHolidays = new USAPublicHoliday();

        var today = DateTime.UtcNow.ConvertUtcToLocalTime(tzInfo).Date;
        var start = request.IncludeToday ? today : today.AddDays(1);
        var end = start.AddDays(days);

        var startUtc = start.ConvertLocalTimeToUtc(tzInfo);
        var endUtc = end.AddDays(1).AddSeconds(-1).ConvertLocalTimeToUtc(tzInfo);

        // -------------------------------------------
        // 1) Single optimized query with projections
        // -------------------------------------------
        var technicians = await db.Set<Technician>()
            .Where(t => t.IsActive
                && t.WorkSchedules.Any(ws => ws.IsActive)
                && t.ServiceZones.Any(tsz => tsz.ServiceZone.IsActive
                    && tsz.ServiceZone.ZipCodes.Contains(request.ZipCode))
                && request.Specialties.All(categoryId => t.Specialities.Any(s => s.ServiceCategoryId == categoryId)))
            .Select(t => new TechnicianSlotData
            (
                t.Id,
                t.WorkSchedules
                    .Where(ws => ws.IsActive)
                    .Select(ws => new WorkScheduleData(ws.Day, ws.StartTime, ws.EndTime))
                    .ToList(),
                t.CalendarBlocks
                    .Where(cb => cb.IsActive)
                    .Select(cb => new CalendarBlockData(cb.SpecificDate, cb.WeeklyDay, cb.AllDay, cb.StartTime, cb.EndTime))
                    .ToList(),
                t.Appointments
                    .Where(av => av.Appointment.ScheduledStart >= startUtc && av.Appointment.ScheduledStart <= endUtc)
                    .Select(av => new AppointmentData(av.Appointment.ScheduledStart, av.Appointment.ScheduledEnd))
                    .ToList()
            ))
            .AsNoTracking()
            .ToListAsync(ct);

        if (technicians.Count == 0)
            return [];

        var currentLocalTime = DateTime.UtcNow.ConvertUtcToLocalTime(tzInfo);

        // Precalculate holidays for the entire range
        var holidaySet = PrecomputeHolidays(usaHolidays, start, end);

        // -------------------------------------------
        // 2) Sequential processing
        // -------------------------------------------
        var dateRange = EachDay(start, end).Where(d => !holidaySet.Contains(d.Date)).ToList();

        var result = new Dictionary<string, List<string>>(dateRange.Count);

        foreach (var date in dateRange)
        {
            var dailySlots = new HashSet<string>(StringComparer.Ordinal);
            var isToday = date.Date == currentLocalTime.Date;

            foreach (var tech in technicians)
            {
                var scheduleRanges = tech.WorkSchedules
                    .Where(ws => ws.Day == date.DayOfWeek)
                    .Select(ws => (ws.StartTime, ws.EndTime))
                    .ToList();

                if (scheduleRanges.Count == 0)
                    continue;

                // Apply blocks
                var blocks = tech.CalendarBlocks
                    .Where(cb => (cb.SpecificDate.HasValue && cb.SpecificDate.Value == DateOnly.FromDateTime(date))
                        || (cb.WeeklyDay.HasValue && cb.WeeklyDay.Value == date.DayOfWeek))
                    .ToList();

                scheduleRanges = ApplyBlocks(scheduleRanges, blocks);

                if (scheduleRanges.Count == 0)
                    continue;

                // Apply appointments
                var appointments = tech.Appointments
                    .Where(a => a.ScheduledStart.ConvertUtcToLocalTime(tzInfo).Date == date.Date)
                    .Select(a =>
                    {
                        var localStart = a.ScheduledStart.ConvertUtcToLocalTime(tzInfo);
                        var localEnd = a.ScheduledEnd?.ConvertUtcToLocalTime(tzInfo)
                            ?? localStart.AddHours(apptDuration);
                        return (localStart.TimeOfDay, localEnd.TimeOfDay);
                    })
                    .ToList();

                scheduleRanges = ApplyAppointments(scheduleRanges, appointments);

                // Split into slots
                var slots = SplitIntoSlots(scheduleRanges, slotMinutes);

                // Filter past slots if today
                if (isToday)
                {
                    slots = [.. slots.Where(s => ParseSlotStartTime(s) > currentLocalTime.TimeOfDay)];
                }

                foreach (var slot in slots)
                    dailySlots.Add(slot);
            }

            if (dailySlots.Count != 0)
            {
                result[date.ToString("yyyy-MM-dd")] = [.. dailySlots.OrderBy(x => x, StringComparer.Ordinal)];
            }
        }

        return result;
    }

    // =====================================================================
    // ------------------------- DATA TRANSFER OBJECTS ---------------------
    // =====================================================================

    private record WorkScheduleData(DayOfWeek Day, TimeSpan StartTime, TimeSpan EndTime);
    private record CalendarBlockData(DateOnly? SpecificDate, DayOfWeek? WeeklyDay, bool AllDay, TimeSpan? StartTime, TimeSpan? EndTime);
    private record AppointmentData(DateTime ScheduledStart, DateTime? ScheduledEnd);
    private record TechnicianSlotData(
        int Id,
        List<WorkScheduleData> WorkSchedules,
        List<CalendarBlockData> CalendarBlocks,
        List<AppointmentData> Appointments
    );

    // =====================================================================
    // ------------------------- HELPERS -------------------------
    // =====================================================================

    /// <summary>
    /// Generates each day from start to end inclusive.
    /// </summary>
    private static IEnumerable<DateTime> EachDay(DateTime from, DateTime to)
    {
        for (var day = from; day <= to; day = day.AddDays(1))
            yield return day;
    }

    private static HashSet<DateTime> PrecomputeHolidays(USAPublicHoliday usaHolidays, DateTime start, DateTime end)
    {
        var holidays = new HashSet<DateTime>();
        for (var day = start; day <= end; day = day.AddDays(1))
        {
            if (usaHolidays.IsPublicHoliday(day))
                holidays.Add(day.Date);
        }
        return holidays;
    }

    private static List<(TimeSpan Start, TimeSpan End)> ApplyBlocks(
        List<(TimeSpan Start, TimeSpan End)> ranges,
        List<CalendarBlockData> blocks)
    {
        foreach (var block in blocks)
        {
            if (block.AllDay)
                return [];

            if (block.StartTime is null || block.EndTime is null)
                continue;

            var updated = new List<(TimeSpan Start, TimeSpan End)>(ranges.Count);

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

    private static List<(TimeSpan Start, TimeSpan End)> ApplyAppointments(
        List<(TimeSpan Start, TimeSpan End)> ranges,
        List<(TimeSpan Start, TimeSpan End)> appointments)
    {
        if (appointments.Count == 0)
            return ranges;

        foreach (var (start, end) in appointments)
        {
            var updated = new List<(TimeSpan Start, TimeSpan End)>(ranges.Count);

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

    private static List<string> SplitIntoSlots(
        List<(TimeSpan Start, TimeSpan End)> ranges,
        int minutes)
    {
        var output = new List<string>();
        var increment = TimeSpan.FromMinutes(minutes);

        foreach (var (start, end) in ranges)
        {
            var cursor = start;

            while (cursor + increment <= end)
            {
                var next = cursor + increment;
                output.Add($@"T{cursor:hh\:mm\:ss}-T{next:hh\:mm\:ss}");
                cursor = next;
            }
        }

        return output;
    }

    private static TimeSpan ParseSlotStartTime(string slot)
    {
        var startTimeStr = slot[1..slot.IndexOf('-')];
        return TimeSpan.Parse(startTimeStr);
    }
}
