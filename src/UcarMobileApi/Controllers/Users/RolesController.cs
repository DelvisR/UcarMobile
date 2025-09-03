using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UcarMobileApi.Application.DTOs.Users;
using UcarMobileApi.Application.Services.Security;
using UcarMobileApi.Attributes;

namespace UcarMobileApi.Controllers.Users;

/// <summary>
/// API controller for managing roles.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController(RoleService roleService) : ControllerBase
{
    /// <summary>Gets all roles.</summary>
    [HttpGet]
    [RequirePermission("role.view")]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
        => Ok(await roleService.GetRolesAsync());

    /// <summary>Gets a role by ID.</summary>
    [HttpGet("{id}")]
    [RequirePermission("role.view")]
    public async Task<ActionResult<RoleDto>> GetRole(int id)
    {
        var role = await roleService.GetRoleAsync(id);
        return role == null ? NotFound() : Ok(role);
    }

    /// <summary>Creates a new role.</summary>
    [HttpPost]
    [RequirePermission("role.create")]
    public async Task<ActionResult<RoleDto>> CreateRole(CreateRoleDto dto)
    {
        var role = await roleService.CreateRoleAsync(dto);
        return CreatedAtAction(nameof(GetRole), new { id = role.Id }, role);
    }

    /// <summary>Updates an existing role.</summary>
    [HttpPut("{id}")]
    [RequirePermission("role.edit")]
    public async Task<IActionResult> UpdateRole(int id, UpdateRoleDto dto)
    {
        var cognitoId = User.FindFirst("sub")?.Value ?? "";
        await roleService.UpdateRoleAsync(id, dto, cognitoId);
        return NoContent();
    }

    /// <summary>Deletes a role.</summary>
    [HttpDelete("{id}")]
    [RequirePermission("role.delete")]
    public async Task<IActionResult> DeleteRole(int id)
    {
        await roleService.DeleteRoleAsync(id);
        return NoContent();
    }

    /// <summary>Assigns permissions to a role.</summary>
    [HttpPut("{id}/assign-permissions")]
    [RequirePermission("role.assign_permission")]
    public async Task<IActionResult> AssignPermissions(int id, [FromBody] List<int> permissionIds)
    {
        await roleService.AssignPermissionsAsync(id, permissionIds);
        return NoContent();
    }
}
