using Microsoft.AspNetCore.Authorization;

namespace UcarMobileApi.Authorization;

/// <summary>
/// Represents a requirement for a specific action within the authorization system.
/// </summary>
/// <remarks>
/// This requirement is evaluated by <see cref="ActionHandler"/> and enforced by ASP.NET Core authorization.
/// </remarks>
public class ActionRequirement(string action) : IAuthorizationRequirement
{
    /// <summary>
    /// Represents a system-level action that can be assigned to roles.
    /// </summary>
    /// <remarks>
    /// Actions define the specific actions or resources a user can access 
    /// when associated with their roles.
    /// </remarks>
    public string Action { get; } = action;
}
