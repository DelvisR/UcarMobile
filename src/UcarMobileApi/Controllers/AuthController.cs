using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UcarMobileApi.Application.DTOs.Users;
using UcarMobileApi.Application.Services.Auth;

namespace UcarMobileApi.Controllers;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Registers a new user in the system using CognitoId
    /// </summary>
    /// <param name="userDto">The user data</param>
    /// <param name="ct">Request cancellation token.</param>
    /// <response code="204">No Content.</response>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<UserDto>> RegisterUser(UserDto userDto, CancellationToken ct)
    {
        await authService.RegisterUserAsync(userDto, ct);
        return NoContent();
    }

    /// <summary>
    /// Gets the currently authenticated user info
    /// </summary>
    /// <param name="ct">Request cancellation token.</param>
    /// <response code="200">User info.</response>
    [HttpGet("me")]
    public async Task<ActionResult<object>> GetCurrentUser(CancellationToken ct)
    {
        var cognitoId = User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(cognitoId))
            return Unauthorized();

        var userInfo = await authService.GetCurrentUserAsync(cognitoId, ct);
        return Ok(userInfo);
    }
}
