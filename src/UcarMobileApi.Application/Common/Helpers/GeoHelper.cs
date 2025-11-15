using System;

namespace UcarMobileApi.Application.Common.Helpers;

/// <summary>
/// Simple geographic helper (Haversine formula).
/// </summary>
public static class GeoHelper
{
    private const double EarthRadiusMiles = 3958.8;

    /// <summary>
    /// Returns distance in miles between two coordinate points using Haversine formula.
    /// </summary>
    public static double DistanceMiles(double lat1, double lon1, double lat2, double lon2)
    {
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return EarthRadiusMiles * c;
    }

    private static double ToRadians(double deg) => deg * Math.PI / 180.0;
}
