using Microsoft.AspNetCore.Mvc;
using UcarMobileApi.Application.DTOs.Technicians;
using UcarMobileApi.Application.Services.Technicians;
using UcarMobileApi.Authorization;

namespace UcarMobileApi.Controllers.Technicians;

/// <summary>
/// API controller that manages technician calendar blocks (vacations, training, medical leave, etc.).
/// Provides CRUD operations and activation toggle for calendar blocks.
/// </summary>
[ApiController]
[Route("api/technicians/{technicianId}/blocks")]
[Tags("Technician Calendar Blocks")]
public class TechnicianCalendarBlocksController(TechnicianCalendarService service) : ControllerBase
{
    /// <summary>
    /// Creates a new calendar block for a technician.
    /// Requires 'ACTION_EDIT_TECHNICIAN' action.
    /// </summary>
    /// <param name="technicianId">Technician identifier.</param>
    /// <param name="dto">DTO containing block information.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>201 Created on success.</returns>
    [HttpPost]
    [RequireAction("ACTION_EDIT_TECHNICIAN")]
    [ProducesResponseType(typeof(TechnicalCalendarBlockDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(int technicianId, [FromBody] TechnicalCalendarBlockDto dto, CancellationToken ct)
    {
        dto.TechnicianId = technicianId;
        var created = await service.CreateBlockAsync(dto, ct);
        return CreatedAtAction(nameof(Get), new { technicianId, id = created.Id }, created);
    }

    /// <summary>
    /// Retrieves a specific calendar block by its identifier.
    /// Requires 'ACTION_VIEW_MAIN_MENU_TECHNICIANS' action.
    /// </summary>
    /// <param name="technicianId">Technician identifier.</param>
    /// <param name="id">Block identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The block if found; otherwise 404.</returns>
    [HttpGet("{id}")]
    [RequireAction("ACTION_VIEW_MAIN_MENU_TECHNICIANS")]
    [ProducesResponseType(typeof(TechnicalCalendarBlockDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int technicianId, int id, CancellationToken ct)
    {
        var block = await service.GetBlockAsync(technicianId, id, ct);
        return block == null ? NotFound() : Ok(block);
    }

    /// <summary>
    /// Retrieves all calendar blocks belonging to a technician.
    /// Requires 'ACTION_VIEW_MAIN_MENU_TECHNICIANS' action.
    /// </summary>
    /// <param name="technicianId">Technician identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of calendar blocks.</returns>
    [HttpGet]
    [RequireAction("ACTION_VIEW_MAIN_MENU_TECHNICIANS")]
    [ProducesResponseType(typeof(IEnumerable<TechnicalCalendarBlockDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(int technicianId, CancellationToken ct)
    {
        var blocks = await service.GetAllBlocksAsync(technicianId, ct);
        return Ok(blocks);
    }

    /// <summary>
    /// Updates an existing technician calendar block.
    /// Requires 'ACTION_EDIT_TECHNICIAN' action.
    /// </summary>
    /// <param name="technicianId">Technician identifier.</param>
    /// <param name="id">Block identifier.</param>
    /// <param name="dto">Updated block DTO.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>200 OK with updated entity.</returns>
    [HttpPut("{id}")]
    [RequireAction("ACTION_EDIT_TECHNICIAN")]
    [ProducesResponseType(typeof(TechnicalCalendarBlockDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int technicianId, int id, [FromBody] TechnicalCalendarBlockDto dto, CancellationToken ct)
    {
        dto.TechnicianId = technicianId;
        var updated = await service.UpdateBlockAsync(id, dto, ct);
        return Ok(updated);
    }

    /// <summary>
    /// Deletes a technician calendar block.
    /// Requires 'ACTION_EDIT_TECHNICIAN' action.
    /// </summary>
    /// <param name="technicianId">Technician identifier.</param>
    /// <param name="id">Block identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>204 NoContent on success.</returns>
    [HttpDelete("{id}")]
    [RequireAction("ACTION_EDIT_TECHNICIAN")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int technicianId, int id, CancellationToken ct)
    {
        await service.DeleteBlockAsync(technicianId, id, ct);
        return NoContent();
    }

    /// <summary>
    /// Enables or disables a calendar block.
    /// Requires 'ACTION_EDIT_TECHNICIAN' action.
    /// </summary>
    /// <param name="technicianId">Technician identifier.</param>
    /// <param name="id">Block identifier.</param>
    /// <param name="isActive">Whether the block should become active or inactive.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>204 NoContent on success.</returns>
    [HttpPatch("{id:int}/activation")]
    [RequireAction("ACTION_EDIT_TECHNICIAN")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ToggleActivationAsync(int technicianId, int id, [FromQuery] bool isActive, CancellationToken ct)
    {
        await service.ToggleBlockActivateAsync(technicianId, id, isActive, ct);
        return NoContent();
    }
}
