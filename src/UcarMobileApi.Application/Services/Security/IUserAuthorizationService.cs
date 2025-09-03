using System.Collections.Generic;
using System.Threading.Tasks;
using UcarMobileApi.Core.Entities.Users;

namespace UcarMobileApi.Application.Services.Security;

/// <summary>
/// Defines the contract for authorization services.
/// </summary>
public interface IUserAuthorizationService
{
    /// <summary>
    /// Checks if a user has a specific permission.
    /// </summary>
    /// <param name="cognitoId">The Cognito ID of the user.</param>
    /// <param name="permissionName">The name of the permission to check.</param>
    /// <returns>True if the user has the permission, otherwise false.</returns>
    Task<bool> HasPermissionAsync(string cognitoId, string permissionName);

    /// <summary>
    /// Gets all permissions for a given user.
    /// </summary>
    /// <param name="cognitoId">The Cognito ID of the user.</param>
    /// <returns>A list of permission names.</returns>
    Task<List<string>> GetUserPermissionsAsync(string cognitoId);

    /// <summary>
    /// Gets all roles for a given user.
    /// </summary>
    /// <param name="cognitoId">The Cognito ID of the user.</param>
    /// <returns>A list of role names.</returns>
    Task<List<string>> GetUserRolesAsync(string cognitoId);

    /// <summary>
    /// Checks if the user is the system user.
    /// </summary>
    /// <param name="cognitoId">The Cognito ID of the user.</param>
    /// <returns>True if the user is the system user, otherwise false.</returns>
    bool IsSystemUser(string cognitoId);

    /// <summary>
    /// Gets user by Cognito ID.
    /// </summary>
    /// <param name="cognitoId">The Cognito ID of the user.</param>
    /// <returns>The user entity or null if not found.</returns>
    Task<User?> GetUserByCognitoIdAsync(string cognitoId);
}