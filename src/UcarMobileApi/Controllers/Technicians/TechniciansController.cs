using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UcarMobileApi.Application.Common.Models;
using UcarMobileApi.Application.DTOs.Technicians;
using UcarMobileApi.Application.Services.Technicians;
using UcarMobileApi.Authorization;
using UcarMobileApi.Web.Extensions;

namespace UcarMobileApi.Controllers.Technicians;

/// <summary>
/// API controller for managing technicians.
/// Provides endpoints for CRUD operations on technician entities.
/// </summary>
[ApiController]
[Route("api/technicians")]
public class TechniciansController(TechnicianService technicianService) : ControllerBase
{
    /// <summary>
    /// Gets all technicians with pagination, filtering, and sorting.
    /// Requires 'ACTION_VIEW_MAIN_MENU_TECHNICIANS' action.
    /// </summary>
    /// <param name="query">Query filter containing pagination, filtering, and sorting parameters.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A paginated list of technician DTOs with pagination headers.</returns>
    [HttpGet]
    [RequireAction("ACTION_VIEW_MAIN_MENU_TECHNICIANS")]
    [ProducesResponseType(typeof(IEnumerable<TechnicianDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TechnicianDto>>> GetTechnicians(
        [FromQuery] QueryFilter query,
        CancellationToken ct)
    {
        var (headers, technicianDtos) = await technicianService.GetTechniciansAsync(query, ct);

        return Ok(technicianDtos).WithHeaders(headers);
    }

    /// <summary>
    /// Gets a specific technician by ID.
    /// Requires 'ACTION_VIEW_MAIN_MENU_TECHNICIANS' action.
    /// </summary>
    /// <param name="id">The technician ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The technician DTO if found, otherwise NotFound.</returns>
    [HttpGet("{id:int}")]
    [RequireAction("ACTION_VIEW_MAIN_MENU_TECHNICIANS")]
    [ProducesResponseType(typeof(TechnicianDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TechnicianDto>> GetTechnician(int id, CancellationToken ct)
    {
        var technician = await technicianService.GetTechnicianAsync(id, ct);
        return technician == null ? NotFound() : Ok(technician);
    }

    /// <summary>
    /// Gets all freelance technicians.
    /// Requires 'ACTION_VIEW_MAIN_MENU_TECHNICIANS' action.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of freelance technician DTOs.</returns>
    [HttpGet("freelance")]
    [RequireAction("ACTION_VIEW_MAIN_MENU_TECHNICIANS")]
    [ProducesResponseType(typeof(IEnumerable<TechnicianDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TechnicianDto>>> GetFreelanceTechnicians(CancellationToken ct)
    {
        var technicians = await technicianService.GetFreelanceTechniciansAsync(ct);
        return Ok(technicians);
    }

    /// <summary>
    /// Gets all employee (non-freelance) technicians.
    /// Requires 'ACTION_VIEW_MAIN_MENU_TECHNICIANS' action.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of employee technician DTOs.</returns>
    [HttpGet("employees")]
    [RequireAction("ACTION_VIEW_MAIN_MENU_TECHNICIANS")]
    [ProducesResponseType(typeof(IEnumerable<TechnicianDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TechnicianDto>>> GetEmployeeTechnicians(CancellationToken ct)
    {
        var technicians = await technicianService.GetEmployeeTechniciansAsync(ct);
        return Ok(technicians);
    }

    /// <summary>
    /// Creates a new technician.
    /// Requires 'ACTION_CREATE_TECHNICIAN' action.
    /// </summary>
    /// <param name="technicianDto">The technician DTO to create.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Created status (201) on success.</returns>
    [HttpPost]
    [RequireAction("ACTION_CREATE_TECHNICIAN")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTechnician(
        [FromBody] TechnicianDto technicianDto,
        CancellationToken ct)
    {
        await technicianService.CreateTechnicianAsync(technicianDto, ct);
        return Created();
    }

    /// <summary>
    /// Updates an existing technician.
    /// Requires 'ACTION_EDIT_TECHNICIAN' action.
    /// </summary>
    /// <param name="id">The technician ID.</param>
    /// <param name="dto">The updated technician DTO.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>NoContent status (204) on success.</returns>
    [HttpPut("{id:int}")]
    [RequireAction("ACTION_EDIT_TECHNICIAN")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTechnician(
        int id,
        [FromBody] TechnicianDto dto,
        CancellationToken ct)
    {
        if (id != dto.Id)
        {
            return BadRequest("Id in route and payload do not match.");
        }

        await technicianService.UpdateTechnicianAsync(id, dto, ct);
        return NoContent();
    }

    /// <summary>
    /// Assigns service zones to a technician.
    /// Requires 'ACTION_EDIT_TECHNICIAN' action.
    /// </summary>
    /// <param name="id">The technician ID.</param>
    /// <param name="serviceZones">The list of service zone assignments.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>NoContent status (204) on success.</returns>
    [HttpPut("{id:int}/service-zones")]
    [RequireAction("ACTION_EDIT_TECHNICIAN")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignServiceZones(
        int id,
        [FromBody] List<TechnicianServiceZoneDto> serviceZones,
        CancellationToken ct)
    {
        await technicianService.AssignServiceZonesAsync(id, serviceZones, ct);
        return NoContent();
    }

    /// <summary>
    /// Assigns specialities to a technician.
    /// Requires 'ACTION_EDIT_TECHNICIAN' action.
    /// </summary>
    /// <param name="id">The technician ID.</param>
    /// <param name="specialities">The list of speciality assignments.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>NoContent status (204) on success.</returns>
    [HttpPut("{id:int}/specialities")]
    [RequireAction("ACTION_EDIT_TECHNICIAN")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignSpecialities(
        int id,
        [FromBody] List<TechnicianSpecialityDto> specialities,
        CancellationToken ct)
    {
        await technicianService.AssignSpecialitiesAsync(id, specialities, ct);
        return NoContent();
    }

    /// <summary>
    /// Gets available time slots for a technician.
    /// </summary>
    /// <param name="request">AvailableSlotRequestDto</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A dictionary keyed by date with a list of available slots ("T08:00:00-T08:30:00")</returns>
    [HttpPost("availableSlots")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAvailability([FromBody] AvailableSlotRequestDto request, CancellationToken ct)
    {
        var data = await technicianService.GetAvailableSlotsAsync(request, ct);
        return Ok(data);
    }

    /// <summary>
    /// Finds and returns the nearest available technician based on the request criteria.
    /// </summary>
    /// <remarks>
    /// This endpoint accepts a <see cref="NearestAvailableRequestDto"/> containing search parameters
    /// and returns the closest available technician. If no technician is found, a 404 response is returned.
    /// </remarks>
    /// <response code="200">Technician found and returned in the response body.</response>
    /// <response code="404">No available technician was found.</response>
    [HttpPost("nearestAvailable")]
    [AllowAnonymous]
    public async Task<IActionResult> GetNearestAvailable([FromBody] NearestAvailableRequestDto request, CancellationToken ct)
    {
        var technician = await technicianService.GetNearestAvailableTechnicianAsync(request, ct);
        return technician == null ? NotFound() : Ok(technician);
    }
}
