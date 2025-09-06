using Microsoft.AspNetCore.Authorization;
using UcarMobileApi.Application.Services.Security;

namespace UcarMobileApi.Authorization;

/// <summary>
/// Handles authorization requirements for permissions.
/// </summary>
/// <remarks>
/// This handler validates if the authenticated user has the required permission 
/// by querying the <see cref="IUserAuthorizationService"/>.
/// </remarks>
public class PermissionHandler(IUserAuthorizationService authorizationService) : AuthorizationHandler<PermissionRequirement>
{
    private readonly IUserAuthorizationService _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));

    /// <summary>
    /// Handles a specific <see cref="PermissionRequirement"/> by checking if the current user 
    /// has the required permission.
    /// </summary>
    /// <param name="context">The authorization context containing the user and resource information.</param>
    /// <param name="requirement">The permission requirement to evaluate.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        // Ensure the resource is an HTTP context
        if (context.Resource is not HttpContext httpContext)
            return;

        var user = httpContext.User;

        // Only continue if the user is authenticated
        if (user?.Identity is not { IsAuthenticated: true })
            return;

        // Retrieve the user's Cognito ID (sub claim)
        var cognitoId = user.FindFirst("sub")?.Value;
        if (string.IsNullOrWhiteSpace(cognitoId))
            return;

        // Check if the user has the required permission
        var hasPermission = await _authorizationService.HasPermissionAsync(cognitoId, requirement.Permission, httpContext.RequestAborted);

        if (hasPermission)
        {
            context.Succeed(requirement);
        }
    }
}
