using Microsoft.AspNetCore.Mvc;
using UcarMobileApi.Application.DTOs.Users;
using UcarMobileApi.Application.Services.Security;
using UcarMobileApi.Application.Services.Users;

namespace UcarMobileApi.Controllers.Users;

/// <summary>
/// Provides endpoints to check actions and roles of the currently authenticated user.
/// </summary>
[ApiController]
[Route("api/auth")]
public class UserAuthorizationController(IUserAuthorizationService authService, CurrentUserService currentUser) : ControllerBase
{
    /// <summary>
    /// Checks whether the current user has a specific action.
    /// </summary>
    /// <param name="action">The action key to check.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns><c>true</c> if the user has the action; otherwise <c>false</c>.</returns>
    [HttpGet("has-action/{action}")]
    public async Task<ActionResult<bool>> HasActionsAsync(string action, CancellationToken ct)
    {
        var result = await authService.HasActionAsync(currentUser.AuthProviderId, action, ct);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves all actions assigned to the current user.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of actions with metadata.</returns>
    [HttpGet("actions")]
    public async Task<ActionResult<List<UserActionDto>>> GetUserActionssAsync(CancellationToken ct)
    {
        var result = await authService.GetUserActionsAsync(currentUser.AuthProviderId, ct);
        return Ok(result);
    }

    /// <summary>
    /// Filters a provided list of actions and returns the ones the current user actually has.
    /// </summary>
    /// <param name="actionssToCheck">The list of actions to validate.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of actions the user has from the provided set.</returns>
    [HttpPost("actions/filter")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetUserActionssFromListAsync([FromBody] IEnumerable<string> actionssToCheck, CancellationToken ct)
    {
        var result = await authService.GetUserActionsFromListAsync(currentUser.AuthProviderId, actionssToCheck, ct);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves all actions of the current user within a specific resource.
    /// </summary>
    /// <param name="resource">The resource of actions to filter.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of actions belonging to the given resource.</returns>
    [HttpGet("actions/resource/{resource}")]
    public async Task<ActionResult<List<string>>> GetUserActionssByResourceAsync(string resource, CancellationToken ct)
    {
        var result = await authService.GetUserActionsByResourceAsync(currentUser.AuthProviderId, resource, ct);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves the roles assigned to the current user.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of role identifiers.</returns>
    [HttpGet("roles")]
    public async Task<ActionResult<List<string>>> GetUserRolesAsync(CancellationToken ct)
    {
        var result = await authService.GetUserRolesAsync(currentUser.AuthProviderId, ct);
        return Ok(result);
    }

    /// <summary>
    /// Checks if the current user is a system user (e.g., admin or service account).
    /// </summary>
    /// <returns><c>true</c> if the user is a system user; otherwise <c>false</c>.</returns>
    [HttpGet("is-system-user")]
    public ActionResult<bool> IsSystemUser()
    {
        var result = authService.IsSystemUser(currentUser.AuthProviderId);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves the current user details by the AuthProviderId of the current user.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>User details if found; otherwise 404 Not Found.</returns>
    [HttpGet("current-user")]
    public async Task<ActionResult> GetCurrentUserAsync(CancellationToken ct)
    {
        var user = await authService.GetUserByAuthProviderIdAsync(currentUser.AuthProviderId, ct);
        return user is null ? NotFound() : Ok(user);
    }
}
