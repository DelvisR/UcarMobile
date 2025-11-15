namespace UcarMobileApi.Application.DTOs.Google.Location;

/// <summary>
/// Represents an address suggestion returned by the Google Places Autocomplete API.
/// </summary>
public class AddressSuggestionDto
{
    /// <summary>
    /// The unique identifier of the place (PlaceId) returned by Google Places.
    /// This value can be used later to retrieve detailed information using the Place Details API.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The human-readable address or description of the suggested place.
    /// Corresponds to the "text.text" field in the Autocomplete API response.
    /// </summary>
    public string Address { get; set; } = string.Empty;
}
