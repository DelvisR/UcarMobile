using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UcarMobileApi.Application.DTOs.Users;

namespace UcarMobileApi.Application.Services.Security;

/// <summary>
/// Defines the contract for authorization services.
/// </summary>
public interface IUserAuthorizationService
{
    /// <summary>
    /// Checks if a user has a specific action.
    /// </summary>
    /// <param name="authProviderId">The Cognito ID of the user.</param>
    /// <param name="action">The name of the action to check.</param>
    /// <param name="ct">Cancellation Token</param>
    /// <returns>True if the user has the action, otherwise false.</returns>
    Task<bool> HasActionAsync(string authProviderId, string action, CancellationToken ct);

    /// <summary>
    /// Returns only the actions (from the requested list) that the user actually has.
    /// </summary>
    /// <param name="authProviderId">User's Cognito ID.</param>
    /// <param name="actionsToCheck">List of actions to validate.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of action names the user has (empty list if none).</returns>
    Task<IReadOnlyList<string>> GetUserActionsFromListAsync(string authProviderId, IEnumerable<string> actionsToCheck, CancellationToken ct);

    /// <summary>
    /// Gets all action names assigned to a user for a given resource.
    /// </summary>
    /// <param name="authProviderId">User's Cognito ID.</param>
    /// <param name="resource">Action resource to filter by.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of action names in the specified resource.</returns>
    Task<List<string>> GetUserActionsByResourceAsync(string authProviderId, string resource, CancellationToken ct);

    /// <summary>
    /// Gets all actions for a given user.
    /// </summary>
    /// <param name="authProviderId">The Cognito ID of the user.</param>
    /// <param name="ct">Cancellation Token</param>
    /// <returns>A list of action names.</returns>
    Task<List<UserActionDto>> GetUserActionsAsync(string authProviderId, CancellationToken ct);

    /// <summary>
    /// Gets cached User.Id for a given authProviderId (populates same cache as GetUserActionsAsync).
    /// </summary>
    /// <param name="authProviderId">The Cognito ID of the user.</param>
    /// <param name="ct">Cancellation Token</param>
    /// <returns>User.Id if found, otherwise null.</returns>
    Task<int?> GetUserIdAsync(string authProviderId, CancellationToken ct);

    /// <summary>
    /// Gets all roles for a given user.
    /// </summary>
    /// <param name="authProviderId">The Cognito ID of the user.</param>
    /// <param name="ct">Cancellation Token</param>
    /// <returns>A list of role names.</returns>
    Task<List<string>> GetUserRolesAsync(string authProviderId, CancellationToken ct);

    /// <summary>
    /// Checks if the user is the system user.
    /// </summary>
    /// <param name="authProviderId">The Cognito ID of the user.</param>
    /// <returns>True if the user is the system user, otherwise false.</returns>
    bool IsSystemUser(string authProviderId);

    /// <summary>
    /// Gets user by Cognito ID.
    /// </summary>
    /// <param name="authProviderId">The Cognito ID of the user.</param>
    /// <param name="ct">Cancellation Token</param>
    /// <returns>The user entity or null if not found.</returns>
    Task<UserDto?> GetUserByAuthProviderIdAsync(string authProviderId, CancellationToken ct);
}
