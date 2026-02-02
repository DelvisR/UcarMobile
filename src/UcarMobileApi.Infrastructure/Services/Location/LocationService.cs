using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UcarMobileApi.Application.Common.Helpers;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Application.DTOs.Google.Location;
using UcarMobileApi.Infrastructure.Providers;

namespace UcarMobileApi.Infrastructure.Services.Location;

/// <summary>
/// Service for performing location-based operations using the Google Places API.
/// </summary>
/// <remarks>
/// This service interacts with the Google Places Autocomplete endpoint to retrieve
/// address suggestions based on a user-provided input string.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="LocationService"/> class.
/// </remarks>
/// <param name="httpClient">The HTTP client used for sending requests to external APIs.</param>
/// <param name="googleApiKeyProvider">Factory for retrieving Google API keys.</param>
/// <param name="mapper">AutoMapper instance for converting API models to DTOs.</param>
/// <param name="logger">Logger instance for capturing operational logs.</param>
public class LocationService(HttpClient httpClient, GoogleApiKeyProvider googleApiKeyProvider, IMapper mapper, ILogger<LocationService> logger) : ILocationService
{
    private const string PlaceBaseUrl = "https://places.googleapis.com/v1";
    private const string GeocodeBaseUrl = "https://maps.googleapis.com/maps/api/geocode/json";
    private const string LanguageCode = "en";
    private const string RegionCode = "US";

    /// <summary>
    /// Retrieves address suggestions based on the provided input using Google Places API.
    /// </summary>
    /// <param name="input">The input text to be used for address autocompletion.</param>
    /// <returns>A list of <see cref="AddressSuggestionDto"/> containing suggested addresses.</returns>
    /// <exception cref="ArgumentException">Thrown when the input text is null or empty.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API call fails.</exception>
    public async Task<List<AddressSuggestionDto>> GetAddressSuggestionsAsync(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Input text cannot be null or empty", nameof(input));

        var apiKey = await googleApiKeyProvider.GetGoogleApiKeyAsync();

        // Prepare the request payload
        var requestBody = new
        {
            input,
            languageCode = LanguageCode,
            regionCode = RegionCode
        };

        var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");


        var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{PlaceBaseUrl}/places:autocomplete")
        {
            Content = content
        };

        requestMessage.Headers.Add("X-Goog-Api-Key", apiKey);
        requestMessage.Headers.Add("X-Goog-FieldMask", "suggestions.placePrediction.placeId,suggestions.placePrediction.text.text"); // filter field

        // Execute request
        var response = await httpClient.SendAsync(requestMessage);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            logger.LogError("Google Places API request failed. Status: {StatusCode}, Content: {Content}", response.StatusCode, errorContent);
            return [];
        }

        var responseContent = await response.Content.ReadAsStringAsync();

        // Deserialize API response to internal model
        var googleResponse = JsonConvert.DeserializeObject<GooglePlacesAutocompleteResponse>(responseContent);

        if (googleResponse == null)
        {
            logger.LogWarning("Empty or invalid response received from Google Places API for input: {Input}", input);
            return [];
        }

        // Map internal Google response → Application DTOs using AutoMapper
        var mappedSuggestions = mapper.Map<List<AddressSuggestionDto>>(googleResponse.Suggestions);

        return mappedSuggestions;
    }

    /// <summary>
    /// Retrieves the coordinates (latitude and longitude) of a location given a Google Place ID.
    /// </summary>
    /// <param name="placeId">The unique Google Place ID of the location.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>A <see cref="LocationDto"/> containing the coordinates, or null if not found.</returns>
    public async Task<LocationDto?> GetPlaceLocationAsync(string placeId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(placeId))
            throw new ArgumentException("PlaceId cannot be null or empty", nameof(placeId));

        var apiKey = await googleApiKeyProvider.GetGoogleApiKeyAsync();

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{PlaceBaseUrl}/places/{placeId}");
        requestMessage.Headers.Add("X-Goog-Api-Key", apiKey);
        requestMessage.Headers.Add("X-Goog-FieldMask", "addressComponents.longText,addressComponents.types,location"); // filter field

        var response = await httpClient.SendAsync(requestMessage, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError("Google Place Details API request failed. Status: {StatusCode}, Content: {Content}", response.StatusCode, errorContent);
            return null;
        }

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        // Deserialize to internal model
        var placeDetails = JsonConvert.DeserializeObject<GooglePlaceDetailsResponse>(responseContent);

        if (placeDetails?.Location == null)
        {
            logger.LogWarning("No location found for PlaceId: {PlaceId}", placeId);
            return null;
        }

        var postalCode = placeDetails.AddressComponents.FirstOrDefault(c => c.Types.Contains("postal_code"))?.LongText;
        var tz = TimeZoneHelper.GetTimeZone(placeDetails.Location.Latitude, placeDetails.Location.Longitude);

        // Map to DTO
        var locationDto = new LocationDto(placeDetails.Location.Latitude, placeDetails.Location.Longitude, postalCode ?? string.Empty, tz);

        return locationDto;
    }

    /// <summary>
    /// Retrieves the coordinates (latitude and longitude) of a location given a full address string using Geocoding API.
    /// </summary>
    /// <param name="address">The complete address of the location.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>A <see cref="LocationDto"/> containing the coordinates, or null if the address could not be geocoded.</returns>
    public async Task<LocationDto?> GetCoordinatesFromAddressAsync(string address, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address cannot be null or empty", nameof(address));

        var apiKey = await googleApiKeyProvider.GetGoogleApiKeyAsync();
        var encodedAddress = Uri.EscapeDataString(address);

        var url = $"{GeocodeBaseUrl}?address={encodedAddress}&key={apiKey}";

        var response = await httpClient.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError("Geocoding API request failed. Status: {StatusCode}, Content: {Content}", response.StatusCode, errorContent);
            return null;
        }

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var geocodeResponse = JsonConvert.DeserializeObject<GoogleGeocodeResponse>(responseContent);

        if (geocodeResponse?.Results == null || geocodeResponse.Results.Count == 0)
        {
            logger.LogWarning("No geocoding result found for address: {Address}", address);
            return null;
        }

        var location = geocodeResponse.Results[0].Geometry.Location;
        var postalCode = geocodeResponse.Results[0].AddressComponents.FirstOrDefault(c => c.Types.Contains("postal_code"))?.LongName;

        var tz = TimeZoneHelper.GetTimeZone(location.Lat, location.Lng);

        return new LocationDto(location.Lat, location.Lng, postalCode ?? string.Empty, tz);
    }
}
