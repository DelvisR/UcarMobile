namespace UcarMobileApi.Application.DTOs.Google.Location;

/// <summary>
/// Represents the latitude and longitude of a place returned by Google Places Place Details API.
/// </summary>
public class LocationDto(double lat, double lng, string zip)
{
    /// <summary>
    /// Latitude of the place.
    /// </summary>
    public double Lat { get; set; } = lat;

    /// <summary>
    /// Longitude of the place.
    /// </summary>
    public double Lng { get; set; } = lng;

    public string Zip { get; set; } = zip;
}
