using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UcarMobileApi.Core.Entities.Users;
using UcarMobileApi.Infrastructure.Data;

namespace UcarMobileApi.Application.Services.Security;

/// <summary>
/// Provides authorization services to check user permissions and roles.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserAuthorizationService"/> class.
/// </remarks>
/// <param name="context">The application's database context.</param>
/// <param name="configuration">The application's configuration.</param>
public class UserAuthorizationService(AppDbContext context, IConfiguration configuration) : IUserAuthorizationService
{

    /// <summary>
    /// Checks if a user has a specific permission.
    /// </summary>
    /// <param name="cognitoId">The Cognito ID of the user.</param>
    /// <param name="permissionName">The name of the permission to check.</param>
    /// <returns>True if the user has the permission, otherwise false.</returns>
    public async Task<bool> HasPermissionAsync(string cognitoId, string permissionName)
    {
        // System user has full access
        if (IsSystemUser(cognitoId))
        {
            return true;
        }

        var user = await context.Set<User>()
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(ra => ra.Permission)
            .FirstOrDefaultAsync(u => u.CognitoId == cognitoId && u.IsActive);

        if (user == null)
        {
            return false;
        }

        // Check if the user has the action through their roles
        var hasPermission = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Any(ra => ra.Permission.Name == permissionName);

        return hasPermission;
    }

    /// <summary>
    /// Gets all permissions for a given user.
    /// </summary>
    /// <param name="cognitoId">The Cognito ID of the user.</param>
    /// <returns>A list of permission names.</returns>
    public async Task<List<string>> GetUserPermissionsAsync(string cognitoId)
    {
        // If it's the system user, return all actions
        if (IsSystemUser(cognitoId))
        {
            return await context.Set<Permission>().Select(a => a.Name).ToListAsync();
        }

        var user = await context.Set<User>()
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(ra => ra.Permission)
            .FirstOrDefaultAsync(u => u.CognitoId == cognitoId && u.IsActive);

        if (user == null)
        {
            return new List<string>();
        }

        // Get all user actions through their roles
        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(ra => ra.Permission.Name)
            .Distinct()
            .ToList();

        return permissions;
    }

    /// <summary>
    /// Gets all roles for a given user.
    /// </summary>
    /// <param name="cognitoId">The Cognito ID of the user.</param>
    /// <returns>A list of role names.</returns>
    public async Task<List<string>> GetUserRolesAsync(string cognitoId)
    {
        var roles = await context.Set<User>()
            .Where(u => u.CognitoId == cognitoId)
            .SelectMany(u => u.UserRoles)
            .Select(ur => ur.Role.Name)
            .Distinct()
            .ToListAsync();

        return roles;
    }

    /// <summary>
    /// Checks if the user is the system user.
    /// </summary>
    /// <param name="cognitoId">The Cognito ID of the user.</param>
    /// <returns>True if the user is the system user, otherwise false.</returns>
    public bool IsSystemUser(string cognitoId)
    {
        var systemUserCognitoId = configuration["SystemUser:CognitoId"];
        return !string.IsNullOrEmpty(systemUserCognitoId) && cognitoId == systemUserCognitoId;
    }

    /// <summary>
    /// Gets user by Cognito ID.
    /// </summary>
    /// <param name="cognitoId">The Cognito ID of the user.</param>
    /// <returns>The user entity or null if not found.</returns>
    public async Task<User?> GetUserByCognitoIdAsync(string cognitoId)
    {
        return await context.Set<User>()
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.CognitoId == cognitoId);
    }
}