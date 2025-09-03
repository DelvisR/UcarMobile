
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UcarMobileApi.Application.Services.Security;

namespace UcarMobileApi.Attributes;

public class RequirePermissionAttribute(string permission) : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // Verify that the user is authenticated
        if (!context.HttpContext.User.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Obtain the CognitoId from the JWT token
        var cognitoId = context.HttpContext.User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(cognitoId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Obtain the authorization service
        var authorizationService = context.HttpContext.RequestServices.GetRequiredService<IUserAuthorizationService>();

        // Verify that the user has the required permission
        var hasPermission = await authorizationService.HasPermissionAsync(cognitoId, permission);
        if (!hasPermission)
        {
            context.Result = new ForbidResult();
            return;
        }
    }
}