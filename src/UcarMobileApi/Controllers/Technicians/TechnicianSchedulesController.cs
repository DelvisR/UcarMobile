using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UcarMobileApi.Application.DTOs.Technicians;
using UcarMobileApi.Application.Services.Technicians;
using UcarMobileApi.Authorization;

namespace UcarMobileApi.Controllers.Technicians;

/// <summary>
/// API controller responsible for managing technician work schedules.
/// Provides CRUD operations and activation toggle for schedules.
/// </summary>
[ApiController]
[Route("api/technicians/{technicianId}/schedules")]
[Tags("Technician Schedules")]
public class TechnicianSchedulesController(TechnicianCalendarService service) : ControllerBase
{
    /// <summary>
    /// Creates a new work schedule for a technician.
    /// Requires 'ACTION_EDIT_TECHNICIAN' action.
    /// </summary>
    [HttpPost]
    //[RequireAction("ACTION_EDIT_TECHNICIAN")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TechnicalWorkScheduleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(int technicianId, [FromBody] TechnicalWorkScheduleDto dto, CancellationToken ct)
    {
        dto.TechnicianId = technicianId;
        var created = await service.CreateScheduleAsync(dto, ct);
        return CreatedAtAction(nameof(Get), new { technicianId, id = created.Id }, created);
    }

    /// <summary>
    /// Creates multiple work schedules for a technician in a single bulk operation.
    /// Requires 'ACTION_EDIT_TECHNICIAN' action.
    /// </summary>
    [HttpPost("bulk")]
    //[RequireAction("ACTION_EDIT_TECHNICIAN")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateMany(int technicianId, [FromBody] IEnumerable<TechnicalWorkScheduleDto> dtos, CancellationToken ct)
    {
        await service.CreateManySchedulesAsync(technicianId, dtos, ct);
        return NoContent();
    }

    /// <summary>
    /// Retrieves a specific work schedule.
    /// Requires 'ACTION_VIEW_MAIN_MENU_TECHNICIANS' action.
    /// </summary>
    [HttpGet("{id}")]
    [RequireAction("ACTION_VIEW_MAIN_MENU_TECHNICIANS")]
    [ProducesResponseType(typeof(TechnicalWorkScheduleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int technicianId, int id, CancellationToken ct)
    {
        var sched = await service.GetScheduleAsync(technicianId, id, ct);
        return sched == null ? NotFound() : Ok(sched);
    }

    /// <summary>
    /// Retrieves all schedules belonging to a technician.
    /// Requires 'ACTION_VIEW_MAIN_MENU_TECHNICIANS' action.
    /// </summary>
    [HttpGet]
    [RequireAction("ACTION_VIEW_MAIN_MENU_TECHNICIANS")]
    [ProducesResponseType(typeof(IEnumerable<TechnicalWorkScheduleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(int technicianId, CancellationToken ct)
    {
        var all = await service.GetAllSchedulesAsync(technicianId, ct);
        return Ok(all);
    }

    /// <summary>
    /// Updates a work schedule.
    /// Requires 'ACTION_EDIT_TECHNICIAN' action.
    /// </summary>
    [HttpPut("{id}")]
    [RequireAction("ACTION_EDIT_TECHNICIAN")]
    [ProducesResponseType(typeof(TechnicalWorkScheduleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int technicianId, int id, [FromBody] TechnicalWorkScheduleDto dto, CancellationToken ct)
    {
        dto.TechnicianId = technicianId;
        var updated = await service.UpdateScheduleAsync(id, dto, ct);
        return Ok(updated);
    }

    /// <summary>
    /// Deletes a technician work schedule.
    /// Requires 'ACTION_EDIT_TECHNICIAN' action.
    /// </summary>
    [HttpDelete("{id}")]
    [RequireAction("ACTION_EDIT_TECHNICIAN")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int technicianId, int id, CancellationToken ct)
    {
        await service.DeleteScheduleAsync(technicianId, id, ct);
        return NoContent();
    }

    /// <summary>
    /// Enables or disables a work schedule.
    /// Requires 'ACTION_EDIT_TECHNICIAN' action.
    /// </summary>
    [HttpPatch("{id:int}/activation")]
    [RequireAction("ACTION_EDIT_TECHNICIAN")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ToggleActivationAsync(int technicianId, int id, [FromQuery] bool isActive, CancellationToken ct)
    {
        await service.ToggleScheduleActivateAsync(technicianId, id, isActive, ct);
        return NoContent();
    }
}
