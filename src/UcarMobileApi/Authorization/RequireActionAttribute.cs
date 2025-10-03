using Microsoft.AspNetCore.Authorization;

namespace UcarMobileApi.Authorization;

/// <summary>
/// Custom attribute used to restrict access to an action or controller based on a specific action.
/// </summary>
/// <remarks>
/// Validates that the authenticated user has the required action before executing the request.
/// </remarks>
public class RequireActionAttribute(string action) : AuthorizeAttribute($"Action:{action}");
