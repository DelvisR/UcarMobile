using Microsoft.AspNetCore.Authorization;

namespace UcarMobileApi.Authorization;

/// <summary>
/// Custom attribute used to restrict access to an action or controller based on a specific permission.
/// </summary>
/// <remarks>
/// Validates that the authenticated user has the required permission before executing the request.
/// </remarks>
public class RequirePermissionAttribute(string permission) : AuthorizeAttribute($"Permission:{permission}");