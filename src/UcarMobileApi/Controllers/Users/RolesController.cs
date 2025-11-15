using Microsoft.AspNetCore.Mvc;
using UcarMobileApi.Application.DTOs.Users;
using UcarMobileApi.Application.Services.Users;
using UcarMobileApi.Authorization;

namespace UcarMobileApi.Controllers.Users;

/// <summary>
/// API controller for managing roles.
/// The "ct" parameter is a cancellation token automatically injected from "HttpContext.RequestAborted".
/// You do not need to specify it.
/// </summary>
[ApiController]
[Route("api/roles")]
public class RolesController(RoleService roleService) : ControllerBase
{
    /// <summary>
    /// Gets all roles.
    /// Requires 'ACTION_VIEW_MAIN_MENU_ROLES' action.
    /// </summary>
    /// <param name="ct">Request cancellation token.</param>
    /// <response code="200">Returns the list of roles.</response>
    /// <response code="401">User not authorized.</response>
    /// <response code="403">User does not have action.</response>
    [HttpGet]
    [RequireAction("ACTION_VIEW_MAIN_MENU_ROLES")]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles(CancellationToken ct)
        => Ok(await roleService.GetRolesAsync(ct));

    /// <summary>
    /// Gets a role by ID.
    /// Requires 'ACTION_VIEW_MAIN_MENU_ROLES' action.
    /// </summary>
    /// <param name="id">The role ID.</param>
    /// <param name="ct">Request cancellation token.</param>
    /// <response code="200">RoleDto if found.</response>
    /// <response code="404">NotFound otherwise.</response>
    [HttpGet("{id:int}")]
    [RequireAction("ACTION_VIEW_MAIN_MENU_ROLES")]
    public async Task<ActionResult<RoleDto>> GetRole(int id, CancellationToken ct)
    {
        var role = await roleService.GetRoleAsync(id, ct);
        return role == null ? NotFound() : Ok(role);
    }

    /// <summary>
    /// Creates a new role.
    /// Requires 'ACTION_CREATE_ROLE' action.
    /// </summary>
    /// <param name="dto">The role data.</param>
    /// <param name="ct">Request cancellation token.</param>
    /// <response code="201">Created.</response>
    [HttpPost]
    [RequireAction("ACTION_CREATE_ROLE")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<RoleDto>> CreateRole(RoleDto dto, CancellationToken ct)
    {
        await roleService.CreateRoleAsync(dto, ct);

        return Created();
    }

    /// <summary>
    /// Updates an existing role.
    /// Requires 'ACTION_EDIT_ROLE' action.
    /// </summary>
    /// <param name="id">The role ID.</param>
    /// <param name="dto">The role update data.</param>
    /// <param name="ct">Request cancellation token.</param>
    /// <response code="204">No Content.</response>
    [HttpPut("{id:int}")]
    [RequireAction("ACTION_EDIT_ROLE")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateRole(int id, RoleDto dto, CancellationToken ct)
    {
        if (id != dto.Id) return BadRequest("Id in route and payload do not match.");

        await roleService.UpdateRoleAsync(dto, ct);
        return NoContent();
    }

    /// <summary>
    /// Deletes a role.
    /// Requires 'ACTION_DELETE_ROLE' action.
    /// </summary>
    /// <param name="id">The role ID.</param>
    /// <param name="ct">Request cancellation token.</param>
    /// <response code="204">No Content.</response>
    [HttpDelete("{id:int}")]
    [RequireAction("ACTION_DELETE_ROLE")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteRole(int id, CancellationToken ct)
    {
        await roleService.DeleteRoleAsync(id, ct);
        return NoContent();
    }

    /// <summary>
    /// Assigns actions to a role.
    /// Requires 'ACTION_EDIT_ROLE' action.
    /// </summary>
    /// <param name="id">The role ID.</param>
    /// <param name="actions">List of ActionDto.</param>
    /// <param name="ct">Request cancellation token.</param>
    /// <response code="204">No Content.</response>
    [HttpPut("{id:int}/assign-actions")]
    [RequireAction("ACTION_EDIT_ROLE")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AssignActions(int id, [FromBody] List<ActionDto> actions, CancellationToken ct)
    {
        await roleService.AssignActionsAsync(id, actions, ct);
        return NoContent();
    }
}
