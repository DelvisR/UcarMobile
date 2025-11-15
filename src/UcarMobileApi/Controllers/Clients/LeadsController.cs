using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UcarMobileApi.Application.Common.Models;
using UcarMobileApi.Application.DTOs.Clients;
using UcarMobileApi.Application.Services.Clients;
using UcarMobileApi.Authorization;
using UcarMobileApi.Web.Extensions;

namespace UcarMobileApi.Controllers.Clients;

/// <summary>
/// API controller that exposes endpoints to manage leads.
/// Routes follow REST conventions: GET /api/leads, GET /api/leads/{id}, POST, PUT, DELETE.
/// </summary>
[ApiController]
[Route("api/leads")]
public class LeadsController(LeadService leadService) : ControllerBase
{
    /// <summary>
    /// GET: api/leads
    /// Returns all leads.
    /// Requires 'ACTION_VIEW_MAIN_MENU_LEADS' action.
    /// </summary>
    /// <response code="200">Returns the list of roles.</response>
    /// <response code="401">User not authorized.</response>
    /// <response code="403">User does not have action.</response>
    [HttpGet]
    [RequireAction("ACTION_VIEW_MAIN_MENU_LEADS")]
    public async Task<ActionResult<IEnumerable<LeadDto>>> GetAll([FromQuery] QueryFilter query, CancellationToken ct)
    {
        var (headers, leadDtos) = await leadService.GetAllAsync(query, ct);

        return Ok(leadDtos).WithHeaders(headers);
    }

    /// <summary>
    /// GET: api/leads/{id}
    /// Returns a single lead or 404 if not found.
    /// Requires 'ACTION_VIEW_MAIN_MENU_LEADS' action.
    /// </summary>
    /// <response code="200">RoleDto if found.</response>
    /// <response code="404">NotFound otherwise.</response>
    [HttpGet("{id:int}")]
    [RequireAction("ACTION_VIEW_MAIN_MENU_LEADS")]
    public async Task<ActionResult<LeadDto>> GetById(int id, CancellationToken ct)
    {
        var lead = await leadService.GetByIdAsync(id, ct);
        return lead == null ? NotFound() : Ok(lead);
    }

    /// <summary>
    /// POST: api/leads
    /// Creates a new lead.
    /// </summary>
    /// <response code="201">Created.</response>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<LeadDto>> Create([FromBody] LeadDto dto, CancellationToken ct)
    {
        await leadService.CreateAsync(dto, ct);

        return Created();
    }

    /// <summary>
    /// PUT: api/leads/{id}
    /// Updates an existing lead.
    /// Requires 'ACTION_EDIT_LEAD' action.
    /// </summary>
    /// <response code="204">No Content.</response>
    [HttpPut("{id:int}")]
    [RequireAction("ACTION_EDIT_LEAD")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(int id, [FromBody] LeadDto dto, CancellationToken ct)
    {
        if (id != dto.Id) return BadRequest("Id in route and payload do not match.");

        await leadService.UpdateAsync(dto, ct);

        return NoContent();
    }

    /// <summary>
    /// DELETE: api/leads/{id}
    /// Deletes a lead.
    /// Requires 'ACTION_DELETE_LEAD' action.
    /// </summary>
    /// <response code="204">No Content.</response>
    [HttpDelete("{id:int}")]
    [RequireAction("ACTION_DELETE_LEAD")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await leadService.DeleteAsync(id, ct);

        return NoContent();
    }
}
