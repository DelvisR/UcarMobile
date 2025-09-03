using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UcarMobileApi.Application.DTOs.Users;
using UcarMobileApi.Application.Services.Auth;

namespace UcarMobileApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Registers a new user in the system.
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> RegisterUser(CreateUserDto request)
    {
        var result = await authService.RegisterUserAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Gets the currently authenticated user info.
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<object>> GetCurrentUser()
    {
        var cognitoId = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(cognitoId))
            return Unauthorized();

        var userInfo = await authService.GetCurrentUserAsync(cognitoId);
        return Ok(userInfo);
    }

    /// <summary>
    /// Initializes system with default data (e.g., roles, admin user).
    /// </summary>
    [HttpPost("initialize-system")]
    public async Task<ActionResult> InitializeSystem()
    {
        await authService.InitializeSystemAsync();
        return Ok("System initialized successfully.");
    }
}
