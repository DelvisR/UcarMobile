using Microsoft.AspNetCore.Mvc;
using UcarMobileApi.Application.Common.Models;
using UcarMobileApi.Application.DTOs.Appointments;
using UcarMobileApi.Application.DTOs.Common;
using UcarMobileApi.Application.DTOs.Technicians;
using UcarMobileApi.Application.Services.Appointments;
using UcarMobileApi.Application.Services.Users;
using UcarMobileApi.Authorization;
using UcarMobileApi.Web.Extensions;

namespace UcarMobileApi.Controllers.Appointments;

/// <summary>
/// API controller for managing appointment lifecycle operations.
/// </summary>
[ApiController]
[Route("api/appointments")]
public class AppointmentsController(AppointmentAppService appointmentAppService, CurrentUserService currentUser) : ControllerBase
{
    #region CRUP

    /// <summary>
    /// Gets all appointments with filtering, sorting, and pagination.
    /// Requires 'ACTION_VIEW_MAIN_MENU_APPOINTMENTS' action.
    /// </summary>
    /// <param name="query">The query parameters for filtering, sorting, and pagination.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of appointments and pagination headers.</returns>
    [HttpGet]
    [RequireAction("ACTION_VIEW_MAIN_MENU_APPOINTMENTS")]
    [ProducesResponseType(typeof(IEnumerable<AppointmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointments([FromQuery] QueryFilter query, CancellationToken ct)
    {
        var (headers, dtos) = await appointmentAppService.GetAppointmentsAsync(currentUser.AuthProviderId, query, ct);
        return Ok(dtos).WithHeaders(headers);
    }

    /// <summary>
    /// Creates a new appointment.
    /// Requires 'ACTION_CREATE_APPOINTMENT' action.
    /// </summary>
    /// <param name="dto">The appointment creation data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Returns 201 Created upon successful creation.</returns>
    [HttpPost]
    [RequireAction("ACTION_CREATE_APPOINTMENT")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAppointment([FromBody] AppointmentCreateDto dto, CancellationToken ct)
    {
        await appointmentAppService.CreateAppointmentAsync(dto, ct);
        return Created();
    }

    /// <summary>
    /// Retrieves an appointment by its ID.
    /// Requires 'ACTION_VIEW_MAIN_MENU_APPOINTMENTS' action.
    /// </summary>
    /// <param name="id">The appointment ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The appointment details.</returns>
    [HttpGet("{id:int}")]
    [RequireAction("ACTION_VIEW_MAIN_MENU_APPOINTMENTS")]
    [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAppointment(int id, CancellationToken ct)
    {
        var appointment = await appointmentAppService.GetAppointmentAsync(id, ct);
        return Ok(appointment);
    }

    /// <summary>
    /// Partially updates an existing appointment.
    /// Only the fields provided in the request body will be modified; 
    /// omitted fields will remain unchanged.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the appointment to update.
    /// </param>
    /// <param name="request">
    /// An object containing one or more appointment fields to update.
    /// All properties are optional.
    /// </param>
    /// <param name="ct">
    /// A cancellation token to cancel the operation.
    /// </param>
    /// <returns>
    /// Returns <see cref="StatusCodes.Status204NoContent"/> if the update succeeds.
    /// Returns <see cref="StatusCodes.Status404NotFound"/> if the appointment does not exist.
    /// Returns <see cref="StatusCodes.Status400BadRequest"/> if the request is invalid.
    /// </returns>
    [HttpPatch("{id:int}")]
    [RequireAction("ACTION_EDIT_APPOINTMENT")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAppointment(int id, [FromBody] UpdateAppointmentRequest request, CancellationToken ct)
    {
        await appointmentAppService.UpdateAsync(id, request, ct);
        return NoContent();
    }

    /// <summary>
    /// Deletes an appointment.
    /// Requires 'ACTION_DELETE_APPOINTMENT' action.
    /// </summary>
    /// <param name="id">The appointment ID to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    [NonAction]                                       /////////////// DISABLED /////////////////////////
    [HttpDelete("{id:int}")]
    [RequireAction("ACTION_DELETE_APPOINTMENT")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAppointment(int id, CancellationToken ct)
    {
        await appointmentAppService.DeleteAppointmentAsync(id, ct);
        return NoContent();
    }

    #endregion

    #region Controlled updates

    /// <summary>
    /// Marks an appointment as completed.
    /// Requires 'ACTION_EDIT_APPOINTMENT_FINALIZE'.
    /// </summary>
    [HttpPost("{id:int}/complete")]
    [RequireAction("ACTION_EDIT_APPOINTMENT_FINALIZE")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CompleteAppointment(int id, [FromBody] CompleteAppointmentRequest request, CancellationToken ct)
    {
        await appointmentAppService.CompleteAppointmentAsync(id, request, ct);
        return NoContent();
    }

    /// <summary>
    /// Cancels an appointment (no request body required).
    /// Requires 'ACTION_EDIT_APPOINTMENT_CANCEL'.
    /// </summary>
    /// <summary>
    /// Cancels an appointment. CancellationReason is optional in the request body.
    /// Uses business parameters to determine fees and charging behavior.
    /// </summary>
    [HttpPost("{id:int}/cancel")]
    [RequireAction("ACTION_EDIT_APPOINTMENT_CANCEL")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelAppointment(int id, [FromBody] CancelAppointmentRequest? request, CancellationToken ct)
    {
        await appointmentAppService.CancelAppointmentAsync(id, request, ct);
        return NoContent();
    }

    /// <summary>
    /// Updates the service address (location) of an existing appointment.
    /// </summary>
    [HttpPost("{id:int}/location")]
    [RequireAction("ACTION_EDIT_APPOINTMENT_LOCATION")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAppointmentAddress(int id, [FromBody] AddressInfoDto request, CancellationToken ct)
    {
        await appointmentAppService.UpdateAppointmentAddressAsync(id, request, ct);
        return NoContent();
    }

    /// <summary>
    /// Reschedules an existing appointment.
    /// </summary>
    [HttpPost("{id:int}/reschedule")]
    [RequireAction("ACTION_EDIT_APPOINTMENT_RESCHEDULE")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RescheduleAppointment(int id, [FromBody] RescheduleRequest request, CancellationToken ct)
    {
        await appointmentAppService.RescheduleAppointmentAsync(id, request, ct);
        return NoContent();
    }

    #endregion

    #region Various

    /// <summary>
    /// Generates and downloads the invoice PDF for a specific appointment.
    /// Requires 'ACTION_VIEW_MAIN_MENU_APPOINTMENTS' action.
    /// </summary>
    /// <param name="id">The unique identifier of the appointment.</param>
    /// <param name="ct">Cancellation token to abort the request.</param>
    /// <returns>A PDF file containing the appointment invoice.</returns>
    [HttpGet("{id:int}/invoice")]
    [RequireAction("ACTION_VIEW_MAIN_MENU_APPOINTMENTS")]
    [Produces("application/pdf")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DownloadInvoicePdf(int id, CancellationToken ct)
    {
        var (pdf, fileName) = await appointmentAppService.GenerateAppointmentInvoicePdfAsync(id, ct);

        return File(pdf, "application/pdf", fileName);
    }

    /// <summary>
    /// Gets available technician time slots that can satisfy an existing appointment.
    /// </summary>
    /// <param name="appointmentId">Appointment identifier.</param>
    /// <param name="includeToday">
    /// Indicates whether the availability calculation should include the current day.
    /// </param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Available time slots grouped by date.</returns>
    [HttpGet("{appointmentId:int}/available-slots")]
    [ProducesResponseType(typeof(Dictionary<string, List<string>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAvailableSlotsForAppointment(int appointmentId, [FromQuery] bool includeToday = false, CancellationToken ct = default)
    {
        var data = await appointmentAppService.GetAvailableSlotsAsync(appointmentId, includeToday, ct);

        return Ok(data);
    }

    /// <summary>
    /// Gets available technicians (ordered by distance) that can cover an existing appointment.
    /// </summary>
    /// <param name="appointmentId">Appointment identifier.</param>
    /// <param name="searchRadiusMeters">Optional maximum search radius in meters (query param). Defaults to 50000 if omitted.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of technicians with distance (nearest first).</returns>
    [HttpGet("{appointmentId:int}/available-technicians")]
    [ProducesResponseType(typeof(IEnumerable<TechnicianWithDistanceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAvailableTechniciansForAppointment(int appointmentId, [FromQuery] double? searchRadiusMeters = null, CancellationToken ct = default)
    {
        var technicians = await appointmentAppService.GetAvailableTechniciansAsync(appointmentId, searchRadiusMeters, ct);
        return Ok(technicians);
    }

    #endregion
}
