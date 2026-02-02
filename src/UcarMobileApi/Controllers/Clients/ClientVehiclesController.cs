using Microsoft.AspNetCore.Mvc;
using UcarMobileApi.Application.DTOs.Clients;
using UcarMobileApi.Application.Services.Clients;
using UcarMobileApi.Application.Services.Users;
using UcarMobileApi.Authorization;

namespace UcarMobileApi.Controllers.Clients;

/// <summary>
/// Manages vehicles associated with clients.
/// </summary>
[ApiController]
[Route("api/clients")]
public class ClientVehiclesController(ClientService clientService, CurrentUserService currentUser) : ControllerBase
{
    #region ClientVehicle

    /// <summary>
    /// List all vehicles for a given client.
    /// Requires 'ACTION_VIEW_MENU_CLIENT_VEHICLES' action.
    /// </summary>
    [HttpGet("{clientId:int}/vehicles")]
    [RequireAction("ACTION_VIEW_MENU_CLIENT_VEHICLES")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ClientVehicleDto>>> GetClientVehicles(int clientId, CancellationToken ct)
    {
        var vehicles = await clientService.GetClientVehiclesAsync(clientId, ct);
        return Ok(vehicles);
    }

    /// <summary>
    /// List all vehicles for a current client.
    /// Requires 'ACTION_VIEW_MENU_CLIENT_VEHICLES' action.
    /// </summary>
    [HttpGet("vehicles")]
    [RequireAction("ACTION_VIEW_MENU_CLIENT_VEHICLES")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ClientVehicleDto>>> GetVehicles(CancellationToken ct)
    {
        var vehicles = await clientService.GetVehiclesAsync(currentUser.AuthProviderId, ct);
        return Ok(vehicles);
    }

    /// <summary>
    /// Get a single client vehicle by id.
    /// Requires 'ACTION_VIEW_MENU_CLIENT_VEHICLES' action.
    /// </summary>
    [HttpGet("{clientId:int}/vehicles/{id:int}")]
    [RequireAction("ACTION_VIEW_MENU_CLIENT_VEHICLES")]
    [ProducesResponseType(typeof(ClientVehicleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClientVehicleDto>> GetClientVehicle(int clientId, int id, CancellationToken ct)
    {
        var vehicle = await clientService.GetClientVehicleAsync(clientId, id, ct);
        return vehicle == null ? NotFound() : Ok(vehicle);
    }

    /// <summary>
    /// Get a single vehicle by id for current client.
    /// Requires 'ACTION_VIEW_MENU_CLIENT_VEHICLES' action.
    /// </summary>
    [HttpGet("vehicles/{id:int}")]
    [RequireAction("ACTION_VIEW_MENU_CLIENT_VEHICLES")]
    [ProducesResponseType(typeof(ClientVehicleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClientVehicleDto>> GetVehicle(int id, CancellationToken ct)
    {
        var vehicle = await clientService.GetVehicleAsync(currentUser.AuthProviderId, id, ct);
        return vehicle == null ? NotFound() : Ok(vehicle);
    }

    /// <summary>
    /// Create a new vehicle for the given client.
    /// Requires 'ACTION_CREATE_CLIENT_VEHICLE' action.
    /// Note: Service method returns Created after creation.
    /// </summary>
    [HttpPost("{clientId:int}/vehicles")]
    [RequireAction("ACTION_CREATE_CLIENT_VEHICLE")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateClientVehicle(int clientId, [FromBody] ClientVehicleUpsertDto dto, CancellationToken ct)
    {
        await clientService.CreateClientVehicleAsync(clientId, dto, ct);
        return Created();
    }

    /// <summary>
    /// Create a new vehicle for the current client.
    /// Requires 'ACTION_CREATE_CLIENT_VEHICLE' action.
    /// Note: Service method returns Created after creation.
    /// </summary>
    [HttpPost("vehicles")]
    [RequireAction("ACTION_CREATE_CLIENT_VEHICLE")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateVehicle([FromBody] ClientVehicleUpsertDto dto, CancellationToken ct)
    {
        await clientService.CreateVehicleAsync(currentUser.AuthProviderId, dto, ct);
        return Created();
    }

    /// <summary>
    /// Update an existing client vehicle.
    /// Requires 'ACTION_EDIT_CLIENT_VEHICLE' action.
    /// </summary>
    [HttpPut("{clientId:int}/vehicles/{id:int}")]
    [RequireAction("ACTION_EDIT_CLIENT_VEHICLE")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateClientVehicle(int clientId, int id, [FromBody] ClientVehicleUpsertDto dto, CancellationToken ct)
    {
        if (id != dto.Id) return BadRequest("Id in route and payload do not match.");

        await clientService.UpdateClientVehicleAsync(clientId, id, dto, ct);
        return NoContent();
    }

    /// <summary>
    /// Update an existing vehicle for current client.
    /// Requires 'ACTION_EDIT_CLIENT_VEHICLE' action.
    /// </summary>
    [HttpPut("vehicles/{id:int}")]
    [RequireAction("ACTION_EDIT_CLIENT_VEHICLE")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateVehicle(int id, [FromBody] ClientVehicleUpsertDto dto, CancellationToken ct)
    {
        if (id != dto.Id) return BadRequest("Id in route and payload do not match.");

        await clientService.UpdateVehicleAsync(currentUser.AuthProviderId, id, dto, ct);
        return NoContent();
    }

    /// <summary>
    /// Partially updates an existing client vehicle (by clientId).
    /// Requires 'ACTION_EDIT_CLIENT_VEHICLE' action.
    /// </summary>
    [HttpPatch("{clientId:int}/vehicles/{id:int}")]
    [RequireAction("ACTION_EDIT_CLIENT_VEHICLE")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchClientVehicle(int clientId, int id, [FromBody] ClientVehicleUpdateDto dto, CancellationToken ct)
    {
        await clientService.UpdateClientVehiclePatchAsync(clientId, id, dto, ct);
        return NoContent();
    }

    /// <summary>
    /// Partially updates an existing client vehicle for the current user.
    /// Requires 'ACTION_EDIT_CLIENT_VEHICLE' action.
    /// </summary>
    [HttpPatch("vehicles/{id:int}")]
    [RequireAction("ACTION_EDIT_CLIENT_VEHICLE")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchVehicle(int id, [FromBody] ClientVehicleUpdateDto dto, CancellationToken ct)
    {
        await clientService.UpdateVehiclePatchAsync(currentUser.AuthProviderId, id, dto, ct);
        return NoContent();
    }

    /// <summary>
    /// Delete a client vehicle.
    /// Requires 'ACTION_DELETE_CLIENT_VEHICLE' action.
    /// </summary>
    [HttpDelete("{clientId:int}/vehicles/{id:int}")]
    [RequireAction("ACTION_DELETE_CLIENT_VEHICLE")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteClientVehicle(int clientId, int id, CancellationToken ct)
    {
        await clientService.DeleteClientVehicleAsync(clientId, id, ct);
        return NoContent();
    }

    /// <summary>
    /// Delete a client vehicle for de current client.
    /// Requires 'ACTION_DELETE_CLIENT_VEHICLE' action.
    /// </summary>
    [HttpDelete("vehicles/{id:int}")]
    [RequireAction("ACTION_DELETE_CLIENT_VEHICLE")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteVehicle(int id, CancellationToken ct)
    {
        await clientService.DeleteVehicleAsync(currentUser.AuthProviderId, id, ct);
        return NoContent();
    }

    #endregion
}
