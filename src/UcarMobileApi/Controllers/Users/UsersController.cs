using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UcarMobileApi.Application.DTOs.Users;
using UcarMobileApi.Application.Services.Users;
using UcarMobileApi.Attributes;

namespace UcarMobileApi.Controllers.Users;

/// <summary>
/// API controller for managing users.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController(UserService userService) : ControllerBase
{
    /// <summary>
    /// Gets all users.
    /// Requires 'user.view' permission.
    /// </summary>
    /// <response code="200">Returns the list of users.</response>
    /// <response code="401">User not authorized.</response>
    /// <response code="403">User does not have permission.</response>
    [HttpGet]
    [RequirePermission("user.view")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        => Ok(await userService.GetUsersAsync());

    /// <summary>
    /// Gets a specific user by ID.
    /// Requires 'user.view' permission.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <response code="200">UserDto if found.</response>
    /// <response code="404">NotFound otherwise.</response>
    [HttpGet("{id}")]
    [RequirePermission("user.view")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var user = await userService.GetUserAsync(id);
        return user == null ? NotFound() : Ok(user);
    }

    /// <summary>
    /// Updates a user (basic info and roles).
    /// Requires 'user.edit' permission.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="dto">The user update data.</param>
    /// <returns>NoContent on success; exceptions handled globally.</returns>
    [HttpPut("{id}")]
    [RequirePermission("user.edit")]
    public async Task<IActionResult> UpdateUser(int id, UpdateUserDto dto)
    {
        var cognitoId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        await userService.UpdateUserAsync(id, dto, cognitoId);
        return NoContent();
    }

    /// <summary>
    /// Activates a user.
    /// Requires 'user.deactivate' permission.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <returns>NoContent on success; 404 if user not found.</returns>
    [HttpPut("{id}/activate")]
    [RequirePermission("user.deactivate")]
    public async Task<IActionResult> ActivateUser(int id)
    {
        await userService.ActivateUserAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Deactivates a user.
    /// Requires 'user.deactivate' permission.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <returns>NoContent on success; 404 if user not found.</returns>
    [HttpDelete("{id}/deactivate")]
    [RequirePermission("user.deactivate")]
    public async Task<IActionResult> DeactivateUser(int id)
    {
        await userService.DeactivateUserAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Assigns roles to a user.
    /// Requires 'user.assign_role' permission.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="roleIds">List of role IDs to assign.</param>
    /// <returns>NoContent on success; exceptions handled globally.</returns>
    [HttpPut("{id}/assign-roles")]
    [RequirePermission("user.assign_role")]
    public async Task<IActionResult> AssignRolesToUser(int id, [FromBody] List<int> roleIds)
    {
        var cognitoId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        await userService.AssignRolesAsync(id, roleIds, cognitoId);
        return NoContent();
    }
}
