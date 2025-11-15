using Microsoft.AspNetCore.Mvc;
using UcarMobileApi.Application.Common.Models;
using UcarMobileApi.Application.DTOs.Users;
using UcarMobileApi.Application.Services.Users;
using UcarMobileApi.Authorization;
using UcarMobileApi.Web.Extensions;

namespace UcarMobileApi.Controllers.Users;

/// <summary>
/// API controller that exposes endpoints to manage actions.
/// Routes follow REST conventions: GET /api/actions.
/// </summary>
[ApiController]
[Route("api/actions")]
public class ActionsController(ActionService actionService) : ControllerBase
{
    /// <summary>
    /// GET: api/actions
    /// Returns all actions.
    /// Requires 'ACTION_MANAGE_ROLE_RESOURCE_AND_ACTIONS' action.
    /// </summary>
    /// <response code="200">Returns the list of roles.</response>
    /// <response code="401">User not authorized.</response>
    /// <response code="403">User does not have action.</response>
    [HttpGet]
    [RequireAction("ACTION_MANAGE_ROLE_RESOURCE_AND_ACTIONS")]
    public async Task<ActionResult<IEnumerable<ActionDto>>> GetAll([FromQuery] QueryFilter query, CancellationToken ct)
    {
        var (headers, actionDtos) = await actionService.GetAllAsync(query, ct);

        return Ok(actionDtos).WithHeaders(headers);
    }
}
