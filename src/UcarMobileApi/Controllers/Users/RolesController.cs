using Microsoft.AspNetCore.Authorization;
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
[Route("api/[controller]")]
[Authorize]
public class RolesController(RoleService roleService) : ControllerBase
{
    /// <summary>
    /// Gets all roles.
    /// Requires 'ROLE_VIEW' permission.
    /// </summary>
    /// <param name="ct">Request cancellation token.</param>
    /// <response code="200">Returns the list of roles.</response>
    /// <response code="401">User not authorized.</response>
    /// <response code="403">User does not have permission.</response>
    [HttpGet]
    [RequirePermission("ROLE_VIEW")]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles(CancellationToken ct)
        => Ok(await roleService.GetRolesAsync(ct));

    /// <summary>
    /// Gets a role by ID.
    /// Requires 'ROLE_VIEW' permission.
    /// </summary>
    /// <param name="id">The role ID.</param>
    /// <param name="ct">Request cancellation token.</param>
    /// <response code="200">RoleDto if found.</response>
    /// <response code="404">NotFound otherwise.</response>
    [HttpGet("{id:int}")]
    [RequirePermission("ROLE_VIEW")]
    public async Task<ActionResult<RoleDto>> GetRole(int id, CancellationToken ct)
    {
        var role = await roleService.GetRoleAsync(id, ct);
        return role == null ? NotFound() : Ok(role);
    }

    /// <summary>
    /// Creates a new role.
    /// Requires 'ROLE_CREATE' permission.
    /// </summary>
    /// <param name="dto">The role data.</param>
    /// <param name="ct">Request cancellation token.</param>
    /// <response code="204">No Content.</response>
    [HttpPost]
    [RequirePermission("ROLE_CREATE")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<RoleDto>> CreateRole(RoleDto dto, CancellationToken ct)
    {
        await roleService.CreateRoleAsync(dto, ct);

        return NoContent();
    }

    /// <summary>
    /// Updates an existing role.
    /// Requires 'ROLE_EDIT' permission.
    /// </summary>
    /// <param name="id">The role ID.</param>
    /// <param name="dto">The role update data.</param>
    /// <param name="ct">Request cancellation token.</param>
    /// <response code="204">No Content.</response>
    [HttpPut("{id:int}")]
    [RequirePermission("ROLE_EDIT")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateRole(int id, RoleDto dto, CancellationToken ct)
    {
        await roleService.UpdateRoleAsync(id, dto, ct);
        return NoContent();
    }

    /// <summary>
    /// Deletes a role.
    /// Requires 'ROLE_DELETE' permission.
    /// </summary>
    /// <param name="id">The role ID.</param>
    /// <param name="ct">Request cancellation token.</param>
    /// <response code="204">No Content.</response>
    [HttpDelete("{id:int}")]
    [RequirePermission("ROLE_DELETE")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteRole(int id, CancellationToken ct)
    {
        await roleService.DeleteRoleAsync(id, ct);
        return NoContent();
    }

    /// <summary>
    /// Assigns permissions to a role.
    /// Requires 'ROLE_ASSIGN_PERMISSION' permission.
    /// </summary>
    /// <param name="id">The role ID.</param>
    /// <param name="permissions">List of PermissionDto.</param>
    /// <param name="ct">Request cancellation token.</param>
    /// <response code="204">No Content.</response>
    [HttpPut("{id:int}/assign-permissions")]
    [RequirePermission("ROLE_ASSIGN_PERMISSION")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AssignPermissions(int id, [FromBody] List<PermissionDto> permissions, CancellationToken ct)
    {
        await roleService.AssignPermissionsAsync(id, permissions, ct);
        return NoContent();
    }
}
