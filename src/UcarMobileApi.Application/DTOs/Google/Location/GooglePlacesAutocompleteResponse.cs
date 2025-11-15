using System.Collections.Generic;
using Newtonsoft.Json;

namespace UcarMobileApi.Application.DTOs.Google.Location;

#region Place AutocompleteResponse

public class GooglePlacesAutocompleteResponse
{
    public List<AutocompleteSuggestion> Suggestions { get; set; } = [];
}

public class AutocompleteSuggestion
{
    public PlacePrediction PlacePrediction { get; set; } = new();
}

public class PlacePrediction
{
    public string PlaceId { get; set; } = string.Empty;

    public TextValue Text { get; set; } = new();
}

public class TextValue
{
    public string Text { get; set; } = string.Empty;
}

#endregion

#region Place Details Response
public class GooglePlaceDetailsResponse
{
    public List<PlacesAddressComponent> AddressComponents { get; set; } = [];
    public PlaceLocation Location { get; set; } = new();
}

public class PlacesAddressComponent
{
    public string LongText { get; set; } = string.Empty;
    public List<string> Types { get; set; } = [];
}

public class PlaceLocation
{
    public double Latitude { get; set; }

    public double Longitude { get; set; }
}

#endregion

#region Geocode Response

public class GoogleGeocodeResponse
{
    public List<GeocodeResult> Results { get; set; } = [];

    public string Status { get; set; } = string.Empty;
}

public class GeocodeResult
{
    [JsonProperty("address_components")]
    public List<AddressComponent> AddressComponents { get; set; } = [];
    public Geometry Geometry { get; set; } = new();
}

public class AddressComponent
{
    [JsonProperty("long_name")]
    public string LongName { get; set; } = string.Empty;
    public List<string> Types { get; set; } = [];
}

public class Geometry
{
    public Location Location { get; set; } = new();
}

public class Location
{
    public double Lat { get; set; }

    public double Lng { get; set; }
}

#endregion
