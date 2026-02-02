using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UcarMobileApi.Application.Common.Models;
using UcarMobileApi.Application.DTOs.Clients;
using UcarMobileApi.Application.Services.Clients;
using UcarMobileApi.Authorization;
using UcarMobileApi.Web.Extensions;

namespace UcarMobileApi.Controllers.Clients;

/// <summary>
/// API controller for managing clients.
/// </summary>
[ApiController]
[Route("api/clients")]
public class ClientsController(ClientService clientService) : ControllerBase
{
    /// <summary>
    /// Get all clients.
    /// Requires 'ACTION_VIEW_MAIN_MENU_CLIENTS' action.
    /// </summary>
    [HttpGet]
    [RequireAction("ACTION_VIEW_MAIN_MENU_CLIENTS")]
    public async Task<ActionResult<IEnumerable<LeadDto>>> GetClients([FromQuery] QueryFilter query, CancellationToken ct)
    {
        var (headers, clientDtos) = await clientService.GetClientsAsync(query, ct);

        return Ok(clientDtos).WithHeaders(headers);
    }

    /// <summary>
    /// Get a specific client by ID.
    /// Requires 'ACTION_VIEW_MAIN_MENU_CLIENTS' action.
    /// </summary>
    [HttpGet("{id:int}")]
    [RequireAction("ACTION_VIEW_MAIN_MENU_CLIENTS")]
    public async Task<ActionResult<ClientDto>> GetClient(int id, CancellationToken ct)
    {
        var client = await clientService.GetClientAsync(id, ct);
        return client == null ? NotFound() : Ok(client);
    }

    /// <summary>
    /// Gets a specific client by email address.
    /// </summary>
    [HttpGet("by-email")]
    [AllowAnonymous]
    public async Task<ActionResult<ClientDto>> GetClientByEmail([FromQuery][EmailAddress] string email, CancellationToken ct)
    {
        var client = await clientService.GetClientByEmailAsync(email, ct);
        return client == null ? NotFound() : Ok(client);
    }


    /// <summary>
    /// Create a new client.
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateClient([FromBody] ClientDto clientDto, CancellationToken ct)
    {
        await clientService.CreateClientAsync(clientDto, ct);
        return Created();
    }

    /// <summary>
    /// Update an existing client.
    /// Requires 'ACTION_EDIT_CLIENT' action.
    /// </summary>
    [HttpPut("{id:int}")]
    [RequireAction("ACTION_EDIT_CLIENT")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateClient(int id, [FromBody] ClientDto dto, CancellationToken ct)
    {
        if (id != dto.Id) return BadRequest("Id in route and payload do not match.");

        await clientService.UpdateClientAsync(id, dto, ct);
        return NoContent();
    }
}
