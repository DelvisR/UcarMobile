using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UcarMobileApi.Application.Common.Models;
using UcarMobileApi.Application.DTOs.Services;
using UcarMobileApi.Application.Services;
using UcarMobileApi.Web.Extensions;

namespace UcarMobileApi.Controllers.Services;

/// <summary>
/// Provides estimate and service data related to a specific vehicle.
/// </summary>
[ApiController]
[Route("api/estimated-services")]
[Produces("application/json")]
[Tags("Estimated Services")]
public class EstimatedServicesController(EstimateService estimateService) : ControllerBase
{
    /// <summary>
    /// Retrieves estimates for a specific vehicle, optionally filtered, sorted and paginated.
    /// </summary>
    /// <param name="vehicleId">Vehicle ID to retrieve estimates for.</param>
    /// <param name="query">Pagination, sorting and filtering options.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of service estimates for the specified vehicle.</returns>
    [HttpGet("{vehicleId:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<EstimateDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEstimates(int vehicleId, [FromQuery] QueryFilter query, CancellationToken ct)
    {
        var (headers, estimates) = await estimateService.GetEstimatesAsync(vehicleId, query, ct);
        return Ok(estimates).WithHeaders(headers);
    }

    /// <summary>
    /// Retrieves the most popular services for a given vehicle.
    /// Popularity is based on the ServicePopularity table.
    /// </summary>
    /// <param name="vehicleId">Vehicle ID to retrieve popular services for.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of popular service estimates for the specified vehicle.</returns>
    [HttpGet("{vehicleId:int}/popular")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<EstimateDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPopularServices(int vehicleId, CancellationToken ct = default)
    {
        var estimates = await estimateService.GetPopularEstimateServicesAsync(vehicleId, ct);
        return Ok(estimates);
    }
}
