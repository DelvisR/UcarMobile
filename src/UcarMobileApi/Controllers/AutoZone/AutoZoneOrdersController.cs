using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UcarMobileApi.Infrastructure.Configurations.AutoZone.Models;
using UcarMobileApi.Infrastructure.Services.AutoZone.Interfaces;

namespace UcarMobileApi.Controllers.AutoZone
{
    /// <summary>
    /// Handles HTTP requests related to AutoZone order operations.
    /// </summary>
    /// <remarks>This controller provides endpoints for managing AutoZone orders via the API. It requires an
    /// implementation of IAutoZoneService to function correctly.</remarks>
    /// <param name="autoZoneService">The service used to perform operations on AutoZone orders. Cannot be null.</param>
    [ApiController]
    [Route("api/autozone")]
    [AllowAnonymous]
    public class AutoZoneOrdersController(IAutoZoneService autoZoneService) : ControllerBase
    {
        /// <summary>
        /// Processes a request to retrieve orders based on the specified criteria.
        /// </summary>
        /// <param name="request">The request containing the criteria for retrieving orders. Cannot be null.</param>
        /// <returns>An <see cref="IActionResult"/> containing a <see cref="OrderDto"/> with the retrieved orders if
        /// successful; otherwise, an appropriate error response.</returns>
        [HttpPost("orders")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrders([FromBody] OrderRequestDto request)
        {
            var result = await autoZoneService.GetOrders(request);
            return Ok(result);
        }
    }
}
