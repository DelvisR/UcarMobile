using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Application.DTOs.Google.Location;
using UcarMobileApi.Controllers.Location;
using Xunit;

namespace UcarMobileApi.Tests.Unit.Controllers
{
    /// <summary>
    /// Unit tests for LocationController using mocked ILocationService.
    /// </summary>
    public class LocationControllerTests
    {
        private static LocationController CreateController(out Mock<ILocationService> serviceMock)
        {
            serviceMock = new Mock<ILocationService>();
            var controller = new LocationController(serviceMock.Object);
            return controller;
        }

        [Fact]
        public async Task Autocomplete_Input_Invalid_Returns_400()
        {
            var controller = CreateController(out _);
            var result = await controller.GetAddressAutocomplete("");
            var bad = result as ObjectResult;

            bad.Should().NotBeNull();
            bad!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task Autocomplete_Valid_Returns_200_With_Data()
        {
            var controller = CreateController(out var mock);
            mock.Setup(x => x.GetAddressSuggestionsAsync("street"))
                .ReturnsAsync([new AddressSuggestionDto { Id = "place_1", Address = "Street 1, City" }]);

            var result = await controller.GetAddressAutocomplete("street");
            var ok = result as OkObjectResult;

            ok.Should().NotBeNull();
            var payload = ok!.Value as List<AddressSuggestionDto>;
            payload.Should().NotBeNull().And.HaveCount(1);
        }

        [Fact]
        public async Task PlaceId_Empty_Returns_400()
        {
            var controller = CreateController(out _);
            var result = await controller.GetPlaceLocation("", CancellationToken.None);
            var bad = result as ObjectResult;

            bad.Should().NotBeNull();
            bad!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task PlaceId_Not_Found_Returns_404()
        {
            var controller = CreateController(out var mock);
            mock.Setup(x => x.GetPlaceLocationAsync("no-exist", CancellationToken.None)).ReturnsAsync((LocationDto)null);

            var result = await controller.GetPlaceLocation("no-exist", CancellationToken.None);
            var notFound = result as ObjectResult;

            notFound.Should().NotBeNull();
            notFound!.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [Fact]
        public async Task PlaceId_Valid_Returns_200_With_Coordinates()
        {
            var controller = CreateController(out var mock);
            mock.Setup(x => x.GetPlaceLocationAsync("place_1", CancellationToken.None))
                .ReturnsAsync(new LocationDto(10.5, -20.3, string.Empty));

            var result = await controller.GetPlaceLocation("place_1", CancellationToken.None);
            var ok = result as OkObjectResult;

            ok.Should().NotBeNull();
            var payload = ok!.Value as LocationDto;
            payload.Should().NotBeNull();
            payload!.Lat.Should().Be(10.5);
            payload!.Lng.Should().Be(-20.3);
        }

        [Fact]
        public async Task Geocode_Address_Empty_Returns_400()
        {
            var controller = CreateController(out _);
            var result = await controller.GetCoordinatesFromAddress("", CancellationToken.None);
            var bad = result as ObjectResult;

            bad.Should().NotBeNull();
            bad!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task Geocode_Address_Valid_Returns_200_With_Coordinates()
        {
            var controller = CreateController(out var mock);
            mock.Setup(x => x.GetCoordinatesFromAddressAsync("123 Av.", CancellationToken.None))
                .ReturnsAsync(new LocationDto(1.234, -5.678, string.Empty));

            var result = await controller.GetCoordinatesFromAddress("123 Av.", CancellationToken.None);
            var ok = result as OkObjectResult;

            ok.Should().NotBeNull();
            var payload = ok!.Value as LocationDto;
            payload.Should().NotBeNull();
            payload!.Lat.Should().Be(1.234);
            payload!.Lng.Should().Be(-5.678);
        }
    }
}
