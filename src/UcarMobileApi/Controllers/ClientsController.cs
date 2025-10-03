using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UcarMobileApi.Application.DTOs;
using UcarMobileApi.Application.Services;
using UcarMobileApi.Authorization;

namespace UcarMobileApi.Controllers;

/// <summary>
/// API controller for managing clients.
/// </summary>
[ApiController]
[Route("api/clients")]
public class ClientsController(ClientService clientService) : ControllerBase
{
    /// <summary>
    /// Gets all clients.
    /// Requires 'ACTION_VIEW_MAIN_MENU_CLIENTS' action.
    /// </summary>
    [HttpGet]
    [RequireAction("ACTION_VIEW_MAIN_MENU_CLIENTS")]
    public async Task<ActionResult<IEnumerable<ClientDto>>> GetClients(CancellationToken ct)
        => Ok(await clientService.GetClientsAsync(ct));

    /// <summary>
    /// Gets a specific client by ID.
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
    /// Creates a new client.
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateClient(ClientDto clientDto, CancellationToken ct)
    {
        await clientService.CreateClientAsync(clientDto, ct);
        return Created();
    }

    /// <summary>
    /// Updates an existing client.
    /// Requires 'ACTION_EDIT_CLIENT' action.
    /// </summary>
    [HttpPut("{id:int}")]
    [RequireAction("ACTION_EDIT_CLIENT")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateClient(int id, ClientDto clientDto, CancellationToken ct)
    {
        await clientService.UpdateClientAsync(id, clientDto, ct);
        return NoContent();
    }
}
