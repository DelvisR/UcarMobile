using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UcarMobileApi.Application.DTOs;
using UcarMobileApi.Application.Services;
using UcarMobileApi.Authorization;

namespace UcarMobileApi.Controllers.ServiceZones;

/// <summary>
/// Controller responsible for managing service zones and validating whether an address
/// or coordinates are within a service area.
/// </summary>
[ApiController]
[Route("api/service-zones")]
public class ServiceZonesController(ServiceZoneService zonesService) : ControllerBase
{
    /// <summary>
    /// Retrieves all service zones.
    /// Requires 'ACTION_VIEW_SERVICE_ZONES' action.
    /// </summary>
    [HttpGet]
    [RequireAction("ACTION_VIEW_SERVICE_ZONES")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await zonesService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a service zone by its unique ID.
    /// Requires 'ACTION_VIEW_SERVICE_ZONES' action.
    /// </summary>
    [HttpGet("{id:int}")]
    [RequireAction("ACTION_VIEW_SERVICE_ZONES")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await zonesService.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Creates a new service zone.
    /// Requires 'ACTION_CREATE_SERVICE_ZONE' action.
    /// </summary>
    [HttpPost]
    [RequireAction("ACTION_CREATE_SERVICE_ZONE")]
    public async Task<IActionResult> Create([FromBody] ServiceZoneDto dto, CancellationToken cancellationToken)
    {
        var created = await zonesService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Updates an existing service zone.
    /// Requires 'ACTION_EDIT_SERVICE_ZONE' action.
    /// </summary>
    [HttpPut("{id:int}")]
    [RequireAction("ACTION_EDIT_SERVICE_ZONE")]
    public async Task<IActionResult> Update(int id, [FromBody] ServiceZoneDto dto, CancellationToken cancellationToken)
    {
        var updated = await zonesService.UpdateAsync(id, dto, cancellationToken);
        return updated is null ? NotFound() : Ok(updated);
    }

    // --------------------------------------------------------------------
    // VALIDATION
    // --------------------------------------------------------------------

    /// <summary>
    /// Validates whether a full address text is within a service zone.
    /// </summary>
    [HttpGet("validate/address")]
    [AllowAnonymous]
    public async Task<IActionResult> ValidateByAddress([FromQuery] string address, CancellationToken cancellationToken)
    {
        var result = await zonesService.ValidateAddressAsync(address, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Validates whether a provider-specific address ID is within a service zone.
    /// </summary>
    [HttpGet("validate/address-id")]
    [AllowAnonymous]
    public async Task<IActionResult> ValidateByAddressId([FromQuery] string id, CancellationToken cancellationToken)
    {
        var result = await zonesService.ValidateAddressByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Validates whether explicit coordinates and ZIP code are within a service zone.
    /// </summary>
    [HttpGet("validate/coords")]
    [AllowAnonymous]
    public async Task<IActionResult> ValidateByCoordinates(
        [FromQuery] double lat,
        [FromQuery] double lng,
        [FromQuery] string zip,
        CancellationToken cancellationToken)
    {
        var result = await zonesService.ValidateAddressAsync(lat, lng, zip, cancellationToken);
        return Ok(result);
    }
}
