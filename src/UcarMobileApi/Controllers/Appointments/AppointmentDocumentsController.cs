using Microsoft.AspNetCore.Mvc;
using UcarMobileApi.Application.DTOs.Appointments;
using UcarMobileApi.Application.Services.Appointments;
using UcarMobileApi.Authorization;

namespace UcarMobileApi.Controllers.Appointments;

/// <summary>
/// API controller for managing documents associated with an appointment.
/// </summary>
[Route("api/appointments/{appointmentId:int}/documents")]
public class AppointmentDocumentsController(AppointmentAppService appointmentAppService) : ControllerBase
{
    #region Documents

    /// <summary>
    /// Returns the documents associated with the appointment.
    /// Requires 'ACTION_VIEW_MAIN_MENU_APPOINTMENTS'.
    /// </summary>
    [HttpGet]
    [RequireAction("ACTION_VIEW_MAIN_MENU_APPOINTMENTS")]
    [ProducesResponseType(typeof(IEnumerable<AppointmentDocumentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDocuments(int appointmentId, CancellationToken ct)
    {
        var docs = await appointmentAppService.GetAppointmentDocumentsAsync(appointmentId, ct);
        return Ok(docs);
    }

    /// <summary>
    /// Adds multiple documents to the appointment (server-side upload) and returns the created DTOs.
    /// Requires 'ACTION_EDIT_APPOINTMENT'.
    /// Accepts multipart/form-data with multiple file fields (name="files").
    /// Optional form field 'prefix' to control storage prefix.
    /// </summary>
    [HttpPost]
    [RequireAction("ACTION_EDIT_APPOINTMENT")]
    [ProducesResponseType(typeof(IEnumerable<AppointmentDocumentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddDocuments(int appointmentId, [FromForm] AddAppointmentDocumentsDto dto, CancellationToken ct)
    {
        var created = await appointmentAppService.AddAppointmentDocumentsAsync(appointmentId, dto, ct);
        return Created(string.Empty, created);
    }

    /// <summary>
    /// Deletes a document link from the appointment.
    /// Requires 'ACTION_EDIT_APPOINTMENT'.
    /// </summary>
    [HttpDelete("{storedFileId:int}")]
    [RequireAction("ACTION_EDIT_APPOINTMENT")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDocument(int appointmentId, int storedFileId, CancellationToken ct)
    {
        await appointmentAppService.DeleteAppointmentDocumentAsync(appointmentId, storedFileId, ct);
        return NoContent();
    }

    #endregion
}
