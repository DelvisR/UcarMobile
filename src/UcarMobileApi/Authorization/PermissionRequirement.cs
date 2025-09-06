using Microsoft.AspNetCore.Authorization;

namespace UcarMobileApi.Authorization;

/// <summary>
/// Represents a requirement for a specific permission within the authorization system.
/// </summary>
/// <remarks>
/// This requirement is evaluated by <see cref="PermissionHandler"/> and enforced by ASP.NET Core authorization.
/// </remarks>
public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    /// <summary>
    /// Represents a system-level permission that can be assigned to roles.
    /// </summary>
    /// <remarks>
    /// Permissions define the specific actions or resources a user can access 
    /// when associated with their roles.
    /// </remarks>
    public string Permission { get; } = permission;
}