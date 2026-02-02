using System;
using System.Collections.Generic;
using UcarMobileApi.Core.Entities.Technicians;

namespace UcarMobileApi.Application.DTOs.Technicians;

/// <summary>
/// Request DTO for querying available time slots
/// </summary>
public class AvailableSlotRequestDto
{
    /// <summary>
    /// Latitude of the service location
    /// </summary>
    public double Lat { get; set; }

    /// <summary>
    /// Longitude of the service location
    /// </summary>
    public double Lng { get; set; }

    public string ZipCode { get; set; } = string.Empty;

    /// <summary>
    /// Service category IDs required for this service.
    /// Only technicians qualified in these categories will be considered.
    /// </summary>
    /// <example>[1, 3, 5]</example>
    public List<int> Specialties { get; set; } = [];

    /// <summary>
    /// Whether the computation should include today's date
    /// </summary>
    public bool IncludeToday { get; set; } = false;
}

public class NearestAvailableRequestDto : AvailableSlotRequestDto
{
    public DateTime LocalStar { get; set; }
    public DateTime LocalEnd { get; set; }
    public double SearchRadiusMeters { get; set; } = 50_000;
}

/// <summary>
/// Represents a technician along with its distance (in meters)
/// from the requested service location.
/// </summary>
public sealed record TechnicianWithDistance(Technician Technician, double DistanceMeters);

/// <summary>
/// Represents a technician along with the distance to a given point (in meters)
/// </summary>
public class TechnicianWithDistanceDto
{
    /// <summary>
    /// Technician information
    /// </summary>
    public TechnicianDto Technician { get; set; } = null!;

    /// <summary>
    /// Distance from the service location in meters
    /// </summary>
    public double DistanceMeters { get; set; }
}
