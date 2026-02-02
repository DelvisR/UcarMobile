using System;
using GeoTimeZone;
using TimeZoneConverter;

namespace UcarMobileApi.Application.Common.Helpers;

/// <summary>
/// Helper class for timezone operations based on geographic coordinates
/// </summary>
public static class TimeZoneHelper
{
    /// <summary>
    /// Gets the TimeZone based on geographic coordinates
    /// </summary>
    /// <param name="latitude">Latitude coordinate</param>
    /// <param name="longitude">Longitude coordinate</param>
    /// <returns>IANA timezone ID from coordinates</returns>
    public static string GetTimeZone(double latitude, double longitude)
    {
        try
        {
            // Get IANA timezone ID from coordinates
            return TimeZoneLookup.GetTimeZone(latitude, longitude).Result;
        }
        catch
        {
            // Fallback to Eastern Time if lookup fails
            return "America/Chicago";
        }
    }

    /// <summary>
    /// Gets the TimeZoneInfo based on geographic coordinates
    /// </summary>
    /// <param name="latitude">Latitude coordinate</param>
    /// <param name="longitude">Longitude coordinate</param>
    /// <returns>TimeZoneInfo for the specified location</returns>
    public static TimeZoneInfo GetTimeZoneInfo(double latitude, double longitude)
    {
        try
        {
            // Get IANA timezone ID from coordinates
            var tzIana = TimeZoneLookup.GetTimeZone(latitude, longitude).Result;

            // Convert IANA to Windows timezone ID and get TimeZoneInfo
            var tzInfo = TZConvert.GetTimeZoneInfo(tzIana);

            return tzInfo;
        }
        catch
        {
            // Fallback to Eastern Time if lookup fails
            return TZConvert.GetTimeZoneInfo("America/Chicago");
        }
    }

    /// <summary>
    /// Converts a UTC datetime to the specified local timezone
    /// </summary>
    /// <param name="utcDateTime">UTC datetime to convert</param>
    /// <param name="timeZone">Target timezone</param>
    /// <returns>Datetime in the local timezone</returns>
    public static DateTime ConvertUtcToLocalTime(this DateTime utcDateTime, TimeZoneInfo timeZone)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZone);
    }

    /// <summary>
    /// Converts a local datetime to UTC
    /// </summary>
    /// <param name="localDateTime">Local datetime to convert</param>
    /// <param name="timeZone">Source timezone</param>
    /// <returns>Datetime in UTC</returns>
    public static DateTime ConvertLocalTimeToUtc(this DateTime localDateTime, TimeZoneInfo timeZone)
    {
        return TimeZoneInfo.ConvertTimeToUtc(localDateTime, timeZone);
    }
}
