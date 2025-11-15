using Microsoft.AspNetCore.Mvc;
using UcarMobileApi.Application.Common.Models;
using UcarMobileApi.Application.DTOs.Users;
using UcarMobileApi.Application.Services.Users;
using UcarMobileApi.Authorization;
using UcarMobileApi.Web.Extensions;

namespace UcarMobileApi.Controllers.Users;

/// <summary>
/// API controller for managing users.
/// The "ct" parameter is a cancellation token automatically injected from "HttpContext.RequestAborted".
/// You do not need to specify it.
/// </summary>
[ApiController]
[Route("api/users")]
public class UsersController(UserService userService) : ControllerBase
{
    /// <summary>
    /// Gets all users.
    /// Requires 'ACTION_VIEW_MAIN_MENU_USERS' action.
    /// </summary>
    /// <response code="200">Returns the list of users.</response>
    /// <response code="401">User not authorized.</response>
    /// <response code="403">User does not have action.</response>
    [HttpGet]
    [RequireAction("ACTION_VIEW_MAIN_MENU_USERS")]
    public async Task<ActionResult<IEnumerable<UserAccountDto>>> GetUsers([FromQuery] QueryFilter query, CancellationToken ct)
    {
        var (headers, usersDto) = await userService.GetUsersAsync(query, ct);

        return Ok(usersDto).WithHeaders(headers);
    }

    /// <summary>
    /// Gets a specific user by ID.
    /// Requires 'ACTION_VIEW_MAIN_MENU_USERS' action.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="ct">Request cancellation token.</param>
    /// <response code="200">UserDto if found.</response>
    /// <response code="404">NotFound otherwise.</response>
    [HttpGet("{id:int}")]
    [RequireAction("ACTION_VIEW_MAIN_MENU_USERS")]
    public async Task<ActionResult<UserAccountDto>> GetUser(int id, CancellationToken ct)
    {
        var user = await userService.GetUserAsync(id, ct);
        return user == null ? NotFound() : Ok(user);
    }

    /// <summary>
    /// Creates a new user (basic info and roles).
    /// Requires 'ACTION_CREATE_USER' action.
    /// </summary>
    /// <param name="userDto">The user data.</param>
    /// <param name="ct">Request cancellation token.</param>
    /// <response code="204">No Content.</response>
    [HttpPost]
    [RequireAction("ACTION_CREATE_USER")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateUser(UserAccountDto userDto, CancellationToken ct)
    {
        await userService.CreateUserAsync(userDto, ct);

        return Created();
    }

    /// <summary>
    /// Updates a user (basic info and roles).
    /// Requires 'ACTION_EDIT_USER' action.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="userDto">The user update data.</param>
    /// <param name="ct">Request cancellation token.</param>
    /// <response code="204">No Content.</response>
    [HttpPut("{id}")]
    [RequireAction("ACTION_EDIT_USER")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateUser(int id, UserAccountDto userDto, CancellationToken ct)
    {
        if (id != userDto.Id) return BadRequest("Id in route and payload do not match.");

        await userService.UpdateUserAsync(id, userDto, ct);
        return NoContent();
    }

    /// <summary>
    /// Activates or deactivates a user.
    /// Requires 'ACTION_EDIT_USER' action.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="active">true to activate, false to deactivate.</param>
    /// <param name="ct">Request cancellation token.</param>
    /// <response code="204">No Content.</response>
    [HttpPut("{id:int}/activation/{active:bool}")]
    [RequireAction("ACTION_EDIT_USER")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ActivateUser(int id, bool active, CancellationToken ct)
    {
        await userService.ActivateUserAsync(id, active, ct);
        return NoContent();
    }
}
