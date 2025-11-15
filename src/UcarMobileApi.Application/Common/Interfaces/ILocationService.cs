using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UcarMobileApi.Application.DTOs.Google.Location;

namespace UcarMobileApi.Application.Common.Interfaces;

/// <summary>
/// Interface for location-related services using Google Places and Geocoding APIs.
/// </summary>
public interface ILocationService
{
    /// <summary>
    /// Retrieves address suggestions based on the provided input text.
    /// </summary>
    /// <param name="input">The input text for address autocomplete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of address suggestions.</returns>
    Task<List<AddressSuggestionDto>> GetAddressSuggestionsAsync(string input);

    /// <summary>
    /// Retrieves the coordinates (latitude and longitude) of a location given a Google Place ID.
    /// </summary>
    /// <param name="placeId">The unique Google Place ID of the location.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains <see cref="LocationDto"/> or null if not found.</returns>
    Task<LocationDto?> GetPlaceLocationAsync(string placeId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the coordinates (latitude and longitude) of a location given a full address string using Geocoding API.
    /// </summary>
    /// <param name="address">The complete address of the location.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains <see cref="LocationDto"/> or null if the address could not be geocoded.</returns>
    Task<LocationDto?> GetCoordinatesFromAddressAsync(string address, CancellationToken cancellationToken);
}
