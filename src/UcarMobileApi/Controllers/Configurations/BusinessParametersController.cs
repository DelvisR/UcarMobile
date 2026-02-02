using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UcarMobileApi.Application.DTOs;
using UcarMobileApi.Application.Services;
using UcarMobileApi.Authorization;

namespace UcarMobileApi.Controllers.Configurations;

/// <summary>
/// Controller for managing global business parameters.
/// </summary>
[ApiController]
[Route("api/business-parameters")]
[Tags("Business Parameters")]
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
    /// Partially updates the value of a global business parameter by its key.
    /// Automatically invalidates the cache after update.
    /// Requires 'ACTION_EDIT_BUSINESS_PARAMETER' action.
    /// </summary>
    /// <param name="key">Key of the parameter to update.</param>
    /// <param name="value">New value represented as a string.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>
    /// Returns NoContent if updated successfully.
    /// Returns 404 if the parameter key does not exist.
    /// Returns 400 if the value cannot be converted to the stored ValueType.
    /// </returns>
    [HttpPatch("{key}/value")]
    [RequireAction("ACTION_EDIT_BUSINESS_PARAMETER")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateValue(string key, [FromBody] string value, CancellationToken cancellationToken)
    {
        await service.UpdateAsync(key, value, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Gets the value of a business parameter by its key.
    /// The value is automatically converted based on the stored ValueType.
    /// </summary>
    /// <param name="key">Key of the parameter to retrieve.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>
    /// Returns the parameter value converted to its actual type.
    /// Returns 404 if the key does not exist.
    /// </returns>
    [HttpGet("{key}/value")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<IActionResult> GetValue(string key, CancellationToken cancellationToken)
    {
        var value = await service.GetValueAsync(key, cancellationToken);
        return Ok(value);
    }
}
