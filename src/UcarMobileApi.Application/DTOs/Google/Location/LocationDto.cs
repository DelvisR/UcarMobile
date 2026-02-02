namespace UcarMobileApi.Application.DTOs.Google.Location;

/// <summary>
/// Represents the latitude and longitude of a place returned by Google Places Place Details API.
/// </summary>
public class LocationDto(double lat, double lng, string zip, string tz)
{
    public double Lat { get; set; } = lat;
    public double Lng { get; set; } = lng;

    public string TimeZone { get; set; } = tz;
    public string Zip { get; set; } = zip;
}
