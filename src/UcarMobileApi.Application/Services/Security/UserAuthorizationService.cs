using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using UcarMobileApi.Application.Common;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Application.DTOs.Users;
using UcarMobileApi.Core.Entities.Users;

namespace UcarMobileApi.Application.Services.Security;

/// <summary>
/// Provides methods to check user actions, roles and retrieve authorization data.
/// </summary>
public class UserAuthorizationService(IAppDbContext context, CognitoRootUserOptions cognitoRoot, ICacheService cache, IMapper mapper) : IUserAuthorizationService
{
    // Sliding expiration for cached actions
    private static readonly TimeSpan CacheSlidingExpiration = TimeSpan.FromMinutes(30);

    /// <summary>
    /// Checks if a user has a specific action by its name.
    /// </summary>
    public async Task<bool> HasActionAsync(string authProviderId, string action, CancellationToken ct)
    {
        var actions = await GetUserActionsAsync(authProviderId, ct);
        return actions.Any(p => p.Name == action);
    }

    /// <summary>
    /// Returns only the actions (from the requested list) that the user actually has.
    /// </summary>
    /// <param name="authProviderId">User's Cognito ID.</param>
    /// <param name="actionsToCheck">List of actions to validate.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of action names the user has (empty list if none).</returns>
    public async Task<IReadOnlyList<string>> GetUserActionsFromListAsync(string authProviderId, IEnumerable<string> actionsToCheck, CancellationToken ct)
    {
        // Get all actions assigned to this user
        var userActions = await GetUserActionsAsync(authProviderId, ct);

        // Convert to HashSet for fast lookup (by action name)
        var actionSet = new HashSet<string>(userActions.Select(p => p.Name));

        // Filter only the ones the user has
        var matchingActions = actionsToCheck
            .Distinct() // avoid duplicates
            .Where(p => actionSet.Contains(p))
            .ToList();

        return matchingActions;
    }

    /// <summary>
    /// Retrieves and caches all actions for a user from the database, including resource.
    /// </summary>
    /// <param name="authProviderId">User's Cognito ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of UserActionDto (Name + Resource).</returns>
    public async Task<List<UserActionDto>> GetUserActionsAsync(string authProviderId, CancellationToken ct)
    {
        // Change cache key to v2 to avoid conflicts with old cache
        var cacheKey = $"user_actions_v2:{authProviderId}";

        // Check cache first
        var cached = await cache.GetAsync<List<UserActionDto>>(cacheKey);
        if (cached is not null)
            return cached;

        // Load actions directly from the database (project Name + Resource)
        var actions = await context.Set<User>()
            .AsNoTracking()
            .Where(u => u.AuthProviderId == authProviderId && u.IsActive)
            .SelectMany(u => u.UserRoles)
            .SelectMany(ur => ur.Role.RoleActions)
            .Select(rp => new UserActionDto(
                rp.Action.Name,
                rp.Action.Resource))
            .Distinct() // optional: distinct on Name+Resource
            .ToListAsync(ct);

        // Save to cache (even empty lists if the user does not exist)
        await cache.SetAsync(cacheKey, actions, null, CacheSlidingExpiration);

        return actions;
    }

    /// <summary>
    /// Gets all action names assigned to a user for a given resource.
    /// </summary>
    /// <param name="authProviderId">User's Cognito ID.</param>
    /// <param name="resource">Action resource to filter by.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of action names in the specified resource.</returns>
    public async Task<List<string>> GetUserActionsByResourceAsync(string authProviderId, string resource, CancellationToken ct)
    {
        var userActions = await GetUserActionsAsync(authProviderId, ct);

        return
        [
            ..userActions
                .Where(p => string.Equals(p.Resource, resource, StringComparison.OrdinalIgnoreCase))
                .Select(p => p.Name)
                .Distinct()
        ];
    }


    /// <summary>
    /// Gets all roles for a given user.
    /// </summary>
    public async Task<List<string>> GetUserRolesAsync(string authProviderId, CancellationToken ct)
    {
        return await context.Set<User>()
            .AsNoTracking()
            .Where(u => u.AuthProviderId == authProviderId)
            .SelectMany(u => u.UserRoles)
            .Select(ur => ur.Role.Name)
            .Distinct()
            .ToListAsync(ct);
    }

    /// <summary>
    /// Checks if the user is the system user (Cognito root user).
    /// </summary>
    public bool IsSystemUser(string authProviderId)
    {
        var systemUserAuthProviderId = cognitoRoot.UserRootCognitoId;
        return !string.IsNullOrEmpty(systemUserAuthProviderId) && authProviderId == systemUserAuthProviderId;
    }

    /// <summary>
    /// Gets a user by Cognito ID including roles.
    /// </summary>
    public async Task<UserDto?> GetUserByAuthProviderIdAsync(string authProviderId, CancellationToken ct)
    {
        return await context.Set<User>()
            .AsNoTracking()
            .Where(u => u.AuthProviderId == authProviderId)
            .ProjectTo<CurrentUserDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }
}
