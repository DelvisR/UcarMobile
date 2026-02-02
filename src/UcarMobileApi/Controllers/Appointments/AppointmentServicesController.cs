using Microsoft.AspNetCore.Mvc;
using UcarMobileApi.Application.DTOs.Appointments;
using UcarMobileApi.Application.Services.Appointments;
using UcarMobileApi.Authorization;

namespace UcarMobileApi.Controllers.Appointments;

/// <summary>
/// API controller for managing services associated with a vehicle within an appointment.
/// </summary>
[Route("api/appointments/{appointmentId:int}/vehicles/{vehicleId:int}/services")]
public class AppointmentServicesController(AppointmentAppService appointmentAppService) : ControllerBase
{
    #region Services

    /// <summary>
    /// Lists services for a specific vehicle within an appointment.
    /// Requires 'ACTION_VIEW_MAIN_MENU_APPOINTMENTS' action.
    /// </summary>
    [HttpGet]
    [RequireAction("ACTION_VIEW_MAIN_MENU_APPOINTMENTS")]
    [ProducesResponseType(typeof(IEnumerable<AppointmentServiceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetServices(int appointmentId, int vehicleId, CancellationToken ct)
    {
        var services = await appointmentAppService.GetServicesAsync(appointmentId, vehicleId, ct);
        return Ok(services);
    }

    /// <summary>
    /// Adds a service to a specific vehicle within an appointment.
    /// Requires 'ACTION_EDIT_APPOINTMENT' action.
    /// </summary>
    /// <param name="appointmentId">Appointment ID.</param>
    /// <param name="vehicleId">Vehicle ID.</param>
    /// <param name="dto">The service data.</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpPost]
    [RequireAction("ACTION_EDIT_APPOINTMENT")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddService(int appointmentId, int vehicleId, [FromBody] AppointmentServiceCreateDto dto, CancellationToken ct)
    {
        await appointmentAppService.AddServiceAsync(appointmentId, vehicleId, dto, ct);
        return Created();
    }

    /// <summary>
    /// Updates an existing service in a vehicle.
    /// Requires 'ACTION_EDIT_APPOINTMENT' action.
    /// </summary>
    /// <param name="appointmentId">Appointment ID.</param>
    /// <param name="vehicleId">Vehicle ID.</param>
    /// <param name="serviceId">Service ID to update.</param>
    /// <param name="dto">The updated service data.</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpPut("{serviceId:int}")]
    [RequireAction("ACTION_EDIT_APPOINTMENT")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateService(int appointmentId, int vehicleId, int serviceId, [FromBody] AppointmentServiceUpdateDto dto, CancellationToken ct)
    {
        await appointmentAppService.UpdateServiceAsync(appointmentId, vehicleId, serviceId, dto, ct);
        return NoContent();
    }

    /// <summary>
    /// Removes a service from a vehicle within an appointment.
    /// Requires 'ACTION_EDIT_APPOINTMENT' action.
    /// </summary>
    /// <param name="appointmentId">Appointment ID.</param>
    /// <param name="vehicleId">Vehicle ID.</param>
    /// <param name="serviceId">Service ID to remove.</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpDelete("{serviceId:int}")]
    [RequireAction("ACTION_EDIT_APPOINTMENT")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteService(int appointmentId, int vehicleId, int serviceId, CancellationToken ct)
    {
        await appointmentAppService.DeleteServiceAsync(appointmentId, vehicleId, serviceId, ct);
        return NoContent();
    }

    #endregion
}
