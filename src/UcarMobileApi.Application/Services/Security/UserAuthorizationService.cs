using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UcarMobileApi.Core.Entities.Users;
using UcarMobileApi.Infrastructure.Data;

namespace UcarMobileApi.Application.Services.Security;

public class UserAuthorizationService(AppDbContext context, IConfiguration configuration, ICacheService cache) : IUserAuthorizationService
{
    private static readonly TimeSpan CacheSlidingExpiration = TimeSpan.FromMinutes(30);

    /// <summary>
    /// Checks if a user has a specific permission.
    /// </summary>
    public async Task<bool> HasPermissionAsync(string cognitoId, string permission, CancellationToken ct)
    {
        var permissions = await GetUserPermissionsAsync(cognitoId, ct);
        return permissions.Contains(permission);
    }

    /// <summary>
    /// Checks if a user has multiple permissions at once.
    /// </summary>
    /// <param name="cognitoId">User's Cognito ID.</param>
    /// <param name="permissionsToCheck">List of permissions to validate.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A dictionary where the key is the permission and the value indicates if the user has it.</returns>
    public async Task<Dictionary<string, bool>> HasPermissionsAsync(string cognitoId, IEnumerable<string> permissionsToCheck, CancellationToken ct)
    {
        var userPermissions = await GetUserPermissionsAsync(cognitoId, ct);

        // HashSet is faster for lookups than List.Contains()
        var permissionSet = new HashSet<string>(userPermissions);

        return permissionsToCheck
            .Distinct() // avoid duplicates in the request
            .ToDictionary(
                p => p,
                p => permissionSet.Contains(p)
            );
    }


    /// <summary>
    /// Retrieves and caches all permissions for a user from the database.
    /// </summary>
    public async Task<List<string>> GetUserPermissionsAsync(string cognitoId, CancellationToken ct)
    {
        var cacheKey = $"user_permissions:{cognitoId}";

        // Check cache first
        var cached = await cache.GetAsync<List<string>>(cacheKey);
        if (cached is not null)
            return cached;

        // Load permissions directly from the database (project only strings)
        var permissions = await context.Set<User>()
            .AsNoTracking()
            .Where(u => u.CognitoId == cognitoId && u.IsActive)
            .SelectMany(u => u.UserRoles)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Name)
            .Distinct()
            .ToListAsync(ct);

        // Save to cache (even empty lists if the user does not exist)
        await cache.SetAsync(cacheKey, permissions, null, CacheSlidingExpiration);

        return permissions;
    }

    /// <summary>
    /// Gets all roles for a given user.
    /// </summary>
    public async Task<List<string>> GetUserRolesAsync(string cognitoId, CancellationToken ct)
    {
        return await context.Set<User>()
            .AsNoTracking()
            .Where(u => u.CognitoId == cognitoId)
            .SelectMany(u => u.UserRoles)
            .Select(ur => ur.Role.Name)
            .Distinct()
            .ToListAsync(ct);
    }

    /// <summary>
    /// Checks if the user is the system user.
    /// </summary>
    public bool IsSystemUser(string cognitoId)
    {
        var systemUserCognitoId = configuration["SystemUser:CognitoId"];
        return !string.IsNullOrEmpty(systemUserCognitoId) && cognitoId == systemUserCognitoId;
    }

    /// <summary>
    /// Gets user by Cognito ID.
    /// </summary>
    public async Task<User?> GetUserByCognitoIdAsync(string cognitoId, CancellationToken ct)
    {
        return await context.Set<User>()
            .AsNoTracking()
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.CognitoId == cognitoId, ct);
    }
}
