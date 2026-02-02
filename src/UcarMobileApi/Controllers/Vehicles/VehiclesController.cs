using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UcarMobileApi.Application.DTOs.Vehicles;
using UcarMobileApi.Application.Services;

namespace UcarMobileApi.Controllers.Vehicles;

/// <summary>
/// Provides read-only vehicle information such as years, makes, and models.
/// </summary>
[ApiController]
[Route("api/vehicles")]
[AllowAnonymous]
public class VehiclesController(VehicleService vehicleService) : ControllerBase
{


    /// <summary>
    /// Gets the list of available vehicle years.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of distinct vehicle years.</returns>
    [HttpGet("years")]
    [ProducesResponseType(typeof(IEnumerable<int>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetYears(CancellationToken ct)
    {
        var years = await vehicleService.GetYearsAsync(ct);
        return Ok(years);
    }

    /// <summary>
    /// Gets available makes for a specific vehicle year.
    /// </summary>
    /// <param name="year">The vehicle year.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of makes for the specified year.</returns>
    [HttpGet("{year:int}/makes")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMakes(int year, CancellationToken ct)
    {
        var makes = await vehicleService.GetMakesAsync(year, ct);

        if (!makes.Any())
            return NotFound($"No makes found for year {year}.");

        return Ok(makes);
    }

    /// <summary>
    /// Gets available models for a specific year and make.
    /// </summary>
    /// <param name="year">The vehicle year.</param>
    /// <param name="make">The vehicle make.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of models with submodels and engines.</returns>
    [HttpGet("{year:int}/makes/{make}/models")]
    [ProducesResponseType(typeof(IEnumerable<ModelDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetModels(int year, string make, CancellationToken ct)
    {
        var models = await vehicleService.GetModelsAsync(year, make, ct);

        if (!models.Any())
            return NotFound($"No models found for year {year} and make '{make}'.");

        return Ok(models);
    }
}
