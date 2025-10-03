using System;
using Microsoft.AspNetCore.Http;

namespace UcarMobileApi.Application.Services.Users;

/// <summary>
/// Provides access to the current authenticated user's information.
/// </summary>
public class CurrentUserService(IHttpContextAccessor httpContextAccessor)
{
    /// <summary>
    /// Gets the AuthProviderId (e.g., subject claim "sub") of the current user.
    /// Throws an exception if no authenticated user is found.
    /// </summary>
    public string AuthProviderId
    {
        get
        {
            // var sub = httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value;
            var nameIdentifier = httpContextAccessor.HttpContext?.User.Identity?.Name;
            return nameIdentifier ?? throw new UnauthorizedAccessException("No authenticated user found.");
        }
    }
}
