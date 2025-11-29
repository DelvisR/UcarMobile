using System.Collections.Generic;

namespace UcarMobileApi.Application.DTOs;

public class ServiceZoneDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string BaseAddress { get; set; } = string.Empty;
    public double RadiusMiles { get; set; } = 25.0;
    public double Lat { get; set; }
    public double Lng { get; set; }
    public List<string> ZipCodes { get; set; } = [];
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Result structure returned by address validation.
/// </summary>
public record AddressValidationResult(bool IsInside, double Lat, double Lng, string Zip);
