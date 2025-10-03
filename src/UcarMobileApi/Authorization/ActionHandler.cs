using Microsoft.AspNetCore.Authorization;
using UcarMobileApi.Application.Services.Security;

namespace UcarMobileApi.Authorization;

/// <summary>
/// Handles authorization requirements for actions.
/// </summary>
/// <remarks>
/// This handler validates if the authenticated user has the required action 
/// by querying the <see cref="IUserAuthorizationService"/>.
/// </remarks>
public class ActionHandler(IUserAuthorizationService authorizationService) : AuthorizationHandler<ActionRequirement>
{
    private readonly IUserAuthorizationService _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));

    /// <summary>
    /// Handles a specific <see cref="ActionRequirement"/> by checking if the current user 
    /// has the required action.
    /// </summary>
    /// <param name="context">The authorization context containing the user and resource information.</param>
    /// <param name="requirement">The action requirement to evaluate.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ActionRequirement requirement)
    {
        // Ensure the resource is an HTTP context
        if (context.Resource is not HttpContext httpContext)
            return;

        var user = httpContext.User;

        // Only continue if the user is authenticated
        if (user?.Identity is not { IsAuthenticated: true })
            return;

        // Retrieve the user's Cognito ID (sub claim)
        var authProviderId = user.Identity?.Name;
        if (string.IsNullOrWhiteSpace(authProviderId))
            return;

        // Check if the user has the required action
        var hasAction = await _authorizationService.HasActionAsync(authProviderId, requirement.Action, httpContext.RequestAborted);

        if (hasAction)
        {
            context.Succeed(requirement);
        }
    }
}
