using Microsoft.AspNetCore.Mvc;
using UcarMobileApi.Application.DTOs;
using UcarMobileApi.Application.Services;
using UcarMobileApi.Authorization;

namespace UcarMobileApi.Controllers.Configurations;

/// <summary>
/// Controller for managing global business parameters.
/// 
/// Provides endpoints to:
/// - Retrieve all global parameters (cached in memory for performance)
/// - Update a parameter value by its key (automatically invalidates cache)
/// 
/// This controller interacts with the BusinessParameterService, which handles
/// data retrieval from the database, mapping to DTOs, and cache management.
/// 
/// Typical use case: exposing global settings such as CommercialMarkup,
/// AutoEstimateLabor, DefaultClientRoles, etc., to the API and admin clients.
/// </summary>
[ApiController]
[Route("api/business-parameters")]
public class BusinessParametersController(BusinessParameterService service) : ControllerBase
{
    /// <summary>
    /// Retrieves all global parameters.
    /// Returns a list of DTOs ready for the client.
    /// Uses in-memory cache if available.
    /// Requires 'ACTION_VIEW_MAIN_MENU_BUSINESS_PARAMETER' action.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>List of BusinessParameterDto</returns>
    [HttpGet]
    [RequireAction("ACTION_VIEW_MAIN_MENU_BUSINESS_PARAMETER")]
    public async Task<ActionResult<List<BusinessParameterDto>>> GetAll(CancellationToken cancellationToken)
    {
        var list = await service.GetAllAsync(cancellationToken);
        return Ok(list);
    }

    /// <summary>
    /// Updates the value of a global parameter by its key.
    /// Automatically invalidates the cache after update.
    /// Requires 'ACTION_EDIT_BUSINESS_PARAMETER' action.
    /// </summary>
    /// <param name="key">Key of the parameter to update</param>
    /// <param name="value">New value as a string</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>NoContent if updated successfully, 404 if the key does not exist</returns>
    [HttpPut("{key}")]
    [RequireAction("ACTION_EDIT_BUSINESS_PARAMETER")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(string key, [FromBody] string value, CancellationToken cancellationToken)
    {
        await service.UpdateAsync(key, value, cancellationToken);
        return NoContent();
    }
}
