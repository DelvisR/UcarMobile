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
    public async Task<IActionResult> CreateUser([FromBody] UserAccountDto userDto, CancellationToken ct)
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
    [HttpPut("{id:int}")]
    [RequireAction("ACTION_EDIT_USER")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserAccountDto userDto, CancellationToken ct)
    {
        if (id != userDto.Id) return BadRequest("Id in route and payload do not match.");

        await userService.UpdateUserAsync(id, userDto, ct);
        return NoContent();
    }

    /// <summary>
    /// Uploads a profile image for the specified user.
    /// If an image already exists, it is replaced.
    /// Requires 'ACTION_EDIT_USER' action.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the user.
    /// </param>
    /// <param name="image">
    /// The image file to upload. Supported formats: JPG, PNG, WEBP.
    /// The maximum allowed size is 2 MB.
    /// </param>
    /// <param name="ct">
    /// A cancellation token to cancel the operation.
    /// </param>
    /// <returns>
    /// Returns <see cref="StatusCodes.Status204NoContent"/> when the image is successfully
    /// created or replaced.
    /// </returns>
    [HttpPut("{id:int}/image")]
    [Consumes("multipart/form-data")]
    [RequireAction("ACTION_EDIT_USER")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpsertUserImage(int id, IFormFile image, CancellationToken ct)
    {
        await userService.UpsertUserImageAsync(id, image, ct);
        return NoContent();
    }

    /// <summary>
    /// Deletes the profile image of the specified user.
    /// Requires 'ACTION_EDIT_USER' action.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the user.
    /// </param>
    /// <param name="ct">
    /// A cancellation token to cancel the operation.
    /// </param>
    /// <returns>
    /// Returns <see cref="StatusCodes.Status204NoContent"/> when the image is successfully deleted,
    /// or when the user does not have a profile image.
    /// </returns>
    [HttpDelete("{id:int}/image")]
    [RequireAction("ACTION_EDIT_USER")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUserImage(int id, CancellationToken ct)
    {
        await userService.DeleteUserImageAsync(id, ct);
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

    /// <summary>
    /// Assigns or updates the roles for a specific user.
    /// Requires 'ACTION_EDIT_USER' action.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="roles">List of roles to assign.</param>
    /// <param name="ct">Request cancellation token.</param>
    /// <response code="204">Roles updated successfully.</response>
    /// <response code="404">User not found.</response>
    [HttpPut("{id:int}/roles")]
    [RequireAction("ACTION_EDIT_USER")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AssignRoles(int id, List<RoleDto> roles, CancellationToken ct)
    {
        await userService.AssignRolesAsync(id, roles, ct);
        return NoContent();
    }

}
