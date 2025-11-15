using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Application.DTOs.Google.Location;
using UcarMobileApi.Application.Mapping.Google;
using UcarMobileApi.Infrastructure.Configurations.Settings;
using UcarMobileApi.Infrastructure.Providers;
using UcarMobileApi.Infrastructure.Services.Location;
using Xunit;

namespace UcarMobileApi.Tests.Unit.Location
{
    /// <summary>
    /// Unit tests for LocationService with simulated HttpClient.
    /// </summary>
    public class LocationServiceTests
    {
        private static IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<LocationProfile>();
            });
            return config.CreateMapper();
        }

        private static GoogleApiKeyProvider CreateGoogleApiKeyProvider()
        {
            // Memory configuration with Google: ApiKey
            var inMemory = new[]
            {
                new KeyValuePair<string, string>("Google:ApiKey", "")
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemory).Build();

            var secretProvider = new Mock<ISecretProvider>();
            var options = Options.Create(new AwsSettings
            {
                Region = "us-east-1",
                AccountId = "000000000000",
                Cognito = new CognitoSettings { UserPoolId = "pool", ClientId = "client" }
            });

            return new GoogleApiKeyProvider(secretProvider.Object, configuration, options);
        }

        [Fact]
        public async Task Autocomplete_Must_Return_Mapped_Suggestions()
        {
            // Arrange
            var handler = new FakeHttpHandler(req =>
            {
                if (req.Method == HttpMethod.Post && req.RequestUri!.ToString().Contains("/places:autocomplete"))
                {
                    var payload = new GooglePlacesAutocompleteResponse
                    {
                        Suggestions =
                        [
                            new AutocompleteSuggestion
                            {
                                PlacePrediction = new PlacePrediction
                                {
                                    PlaceId = "ChIJ2eUgeAK6j4ARbn5u_wAGqWA",
                                    Text = new TextValue { Text = "1600 Amphitheatre Parkway, Mountain View, CA, USA" }
                                }
                            }
                        ]
                    };
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(payload))
                    };
                }
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });

            var httpClient = new HttpClient(handler);
            var mapper = CreateMapper();
            var apiKeyProvider = CreateGoogleApiKeyProvider();
            var service = new LocationService(httpClient, apiKeyProvider, mapper, Mock.Of<Microsoft.Extensions.Logging.ILogger<LocationService>>());

            // Act
            var result = await service.GetAddressSuggestionsAsync("1600 Amph");

            // Assert
            result.Should().NotBeNull().And.HaveCount(1);
            result[0].Id.Should().Be("ChIJ2eUgeAK6j4ARbn5u_wAGqWA");
            result[0].Address.Should().Contain("Amphitheatre");
        }

        [Fact]
        public async Task PlaceDetails_Must_Return_Coordinates()
        {
            // Arrange
            var handler = new FakeHttpHandler(req =>
            {
                if (req.Method == HttpMethod.Get && req.RequestUri!.ToString().Contains("/places/ChIJ2eUgeAK6j4ARbn5u_wAGqWA"))
                {
                    var payload = new GooglePlaceDetailsResponse
                    {
                        Location = new PlaceLocation { Latitude = 37.422, Longitude = -122.084 }
                    };
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(payload))
                    };
                }
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });

            var httpClient = new HttpClient(handler);
            var mapper = CreateMapper();
            var apiKeyProvider = CreateGoogleApiKeyProvider();
            var service = new LocationService(httpClient, apiKeyProvider, mapper, Mock.Of<Microsoft.Extensions.Logging.ILogger<LocationService>>());

            // Act
            var result = await service.GetPlaceLocationAsync("ChIJ2eUgeAK6j4ARbn5u_wAGqWA");

            // Assert
            result.Should().NotBeNull();
            result!.Lat.Should().Be(37.422);
            result!.Lng.Should().Be(-122.084);
        }

        [Fact]
        public async Task Geocode_Full_Address_Must_Return_Coordinates()
        {
            // Arrange
            var handler = new FakeHttpHandler(req =>
            {
                if (req.Method == HttpMethod.Get && req.RequestUri!.ToString().Contains("maps.googleapis.com/maps/api/geocode/json"))
                {
                    var payload = new GoogleGeocodeResponse
                    {
                        Status = "OK",
                        Results =
                        [
                            new GeocodeResult
                            {
                                Geometry = new Geometry
                                {
                                    Location = new Application.DTOs.Google.Location.Location { Lat = 37.422, Lng = -122.084 }
                                }
                            }
                        ]
                    };
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(payload))
                    };
                }
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });

            var httpClient = new HttpClient(handler);
            var mapper = CreateMapper();
            var apiKeyProvider = CreateGoogleApiKeyProvider();
            var service = new LocationService(httpClient, apiKeyProvider, mapper, Mock.Of<Microsoft.Extensions.Logging.ILogger<LocationService>>());

            // Act
            var result = await service.GetCoordinatesFromAddressAsync("1600 Amphitheatre Parkway, Mountain View, CA");

            // Assert
            result.Should().NotBeNull();
            result!.Lat.Should().Be(37.422);
            result!.Lng.Should().Be(-122.084);
        }

        /// <summary>
        /// Fake HttpMessageHandler that allows responding based on the Request.
        /// </summary>
        private class FakeHttpHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var response = responder(request);
                return Task.FromResult(response);
            }
        }
    }
}
