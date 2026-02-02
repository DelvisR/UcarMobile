using Microsoft.AspNetCore.Mvc;
using UcarMobileApi.Application.Common.Models;
using UcarMobileApi.Application.DTOs.Appointments;
using UcarMobileApi.Application.Services.Appointments;
using UcarMobileApi.Authorization;
using UcarMobileApi.Web.Extensions;

namespace UcarMobileApi.Controllers.Appointments;

/// <summary>
/// API controller for managing notes associated with an appointment.
/// </summary>
[Route("api/appointments/{appointmentId:int}/notes")]
public class AppointmentNotesController(AppointmentAppService appointmentAppService) : ControllerBase
{
    #region Notes

    /// <summary>
    /// Returns notes for the appointment.
    /// Requires 'ACTION_VIEW_MAIN_MENU_APPOINTMENTS'.
    /// </summary>
    [HttpGet]
    [RequireAction("ACTION_VIEW_MAIN_MENU_APPOINTMENTS")]
    [ProducesResponseType(typeof(IEnumerable<AppointmentNoteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetNotes(int appointmentId, [FromQuery] QueryFilter query, CancellationToken ct)
    {
        var (headers, notes) = await appointmentAppService.GetAppointmentNotesAsync(appointmentId, query, ct);

        return Ok(notes).WithHeaders(headers);
    }

    /// <summary>
    /// Adds a note to the appointment. Accepts optional files (multipart/form-data).
    /// Requires 'ACTION_EDIT_APPOINTMENT'.
    /// </summary>
    [HttpPost]
    [RequireAction("ACTION_EDIT_APPOINTMENT")]
    [ProducesResponseType(typeof(AppointmentNoteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddNote(int appointmentId, [FromForm] AppointmentNoteCreateDto request, CancellationToken ct)
    {
        var created = await appointmentAppService.AddAppointmentNoteAsync(appointmentId, request, ct);
        return Created(string.Empty, created);
    }

    /// <summary>
    /// Deletes a note from an appointment.
    /// Requires 'ACTION_EDIT_APPOINTMENT'.
    /// </summary>
    [HttpDelete("{noteId:int}")]
    [RequireAction("ACTION_EDIT_APPOINTMENT")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteNote(int id, int noteId, CancellationToken ct)
    {
        await appointmentAppService.DeleteAppointmentNoteAsync(id, noteId, ct);
        return NoContent();
    }

    #endregion
}
