using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Application.DTOs.Google.Location;

namespace UcarMobileApi.Controllers.Location;

/// <summary>
/// Controller for location-related operations, including address autocomplete functionality.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="LocationController"/> class.
/// </remarks>
/// <param name="locationService">The location service responsible for address lookup operations.</param>
[ApiController]
[Route("api/locations")]
[Produces("application/json")]
[AllowAnonymous]
public class LocationController(ILocationService locationService) : ControllerBase
{
    /// <summary>
    /// Retrieves address suggestions based on the provided input text using Google Places API.
    /// </summary>
    /// <param name="input">The input text for address autocomplete.</param>
    /// <returns>A list of address suggestions with formatted addresses and location coordinates.</returns>
    /// <response code="200">Returns the list of address suggestions.</response>
    /// <response code="400">If the input text is null or empty.</response>
    [HttpGet("autocomplete")]
    [ProducesResponseType(typeof(List<AddressSuggestionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAddressAutocomplete([FromQuery] string input)
    {
        if (string.IsNullOrWhiteSpace(input) || input.Length < 2)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid Request",
                Detail = "The input text cannot be null or empty and must be at least 2 characters long.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var suggestions = await locationService.GetAddressSuggestionsAsync(input);

        return Ok(suggestions);
    }

    /// <summary>
    /// Retrieves the latitude and longitude for a given PlaceId.
    /// </summary>
    /// <param name="placeId">The PlaceId returned by Autocomplete.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>A <see cref="LocationDto"/> with Lat and Lon.</returns>
    /// <response code="200">Returns the place location.</response>
    /// <response code="400">If the PlaceId is null or empty.</response>
    [HttpGet("geocode/place-id/{placeId}")]
    [ProducesResponseType(typeof(LocationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPlaceLocation([FromRoute] string placeId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(placeId))
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid Request",
                Detail = "PlaceId cannot be null or empty.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var location = await locationService.GetPlaceLocationAsync(placeId, cancellationToken);

        if (location == null)
            return NotFound(new ProblemDetails
            {
                Title = "Place Not Found",
                Detail = $"No location found for PlaceId: {placeId}",
                Status = StatusCodes.Status404NotFound
            });

        return Ok(location);
    }

    /// <summary>
    /// Retrieves the latitude and longitude of a location given a full address string.
    /// </summary>
    /// <param name="address">The complete address to geocode.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>A <see cref="LocationDto"/> containing latitude and longitude, or 404 if not found.</returns>
    /// <response code="200">Returns the location coordinates.</response>
    /// <response code="400">If the address is null, empty, or invalid.</response>
    /// <response code="404">If the address could not be geocoded.</response>
    [HttpGet("geocode/address/{address}")]
    [ProducesResponseType(typeof(LocationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCoordinatesFromAddress([FromRoute] string address, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid Request",
                Detail = "The address cannot be null or empty.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var location = await locationService.GetCoordinatesFromAddressAsync(address, cancellationToken);

        if (location == null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Address Not Found",
                Detail = $"No location could be found for address: {address}",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(location);
    }

}
