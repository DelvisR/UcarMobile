using Microsoft.AspNetCore.Mvc;
using UcarMobileApi.Application.DTOs.Users;
using UcarMobileApi.Application.Services.Users;
using UcarMobileApi.Authorization;

namespace UcarMobileApi.Controllers.Users;

/// <summary>
/// API controller for managing users.
/// The "ct" parameter is a cancellation token automatically injected from "HttpContext.RequestAborted".
/// You do not need to specify it.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController(UserService userService) : ControllerBase
{
    /// <summary>
    /// Gets all users.
    /// Requires 'USER_VIEW' permission.
    /// </summary>
    /// <param name="ct">Request cancellation token.</param>
    /// <response code="200">Returns the list of users.</response>
    /// <response code="401">User not authorized.</response>
    /// <response code="403">User does not have permission.</response>
    [HttpGet]
    [RequirePermission("USER_VIEW")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers(CancellationToken ct)
        => Ok(await userService.GetUsersAsync(ct));

    /// <summary>
    /// Gets a specific user by ID.
    /// Requires 'USER_VIEW' permission.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="ct">Request cancellation token.</param>
    /// <response code="200">UserDto if found.</response>
    /// <response code="404">NotFound otherwise.</response>
    [HttpGet("{id:int}")]
    [RequirePermission("USER_VIEW")]
    public async Task<ActionResult<UserDto>> GetUser(int id, CancellationToken ct)
    {
        var user = await userService.GetUserAsync(id, ct);
        return user == null ? NotFound() : Ok(user);
    }

    /// <summary>
    /// Creates a new user (basic info and roles).
    /// Requires 'USER_CREATE' permission.
    /// </summary>
    /// <param name="userDto">The user data.</param>
    /// <param name="ct">Request cancellation token.</param>
    /// <response code="204">No Content.</response>
    [HttpPost]
    [RequirePermission("USER_CREATE")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CreateUser(UserDto userDto, CancellationToken ct)
    {
        await userService.CreateUserAsync(userDto, ct);

        return NoContent();
    }

    /// <summary>
    /// Updates a user (basic info and roles).
    /// Requires 'USER_EDIT' permission.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="userDto">The user update data.</param>
    /// <param name="ct">Request cancellation token.</param>
    /// <response code="204">No Content.</response>
    [HttpPut("{id}")]
    [RequirePermission("USER_EDIT")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateUser(int id, UserDto userDto, CancellationToken ct)
    {
        await userService.UpdateUserAsync(id, userDto, ct);
        return NoContent();
    }

    /// <summary>
    /// Activates or deactivates a user.
    /// Requires 'USER_EDIT' permission.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="active">true to activate, false to deactivate.</param>
    /// <param name="ct">Request cancellation token.</param>
    /// <response code="204">No Content.</response>
    [HttpPut("{id:int}/activation/{active:bool}")]
    [RequirePermission("USER_EDIT")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ActivateUser(int id, bool active, CancellationToken ct)
    {
        await userService.ActivateUserAsync(id, active, ct);
        return NoContent();
    }
}
