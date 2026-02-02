using Microsoft.AspNetCore.Mvc;
using UcarMobileApi.Application.DTOs.Appointments;
using UcarMobileApi.Application.Services.Appointments;
using UcarMobileApi.Authorization;

namespace UcarMobileApi.Controllers.Appointments;

/// <summary>
/// API controller for managing parts associated with a service within an appointment.
/// </summary>
[Route("api/appointments/{appointmentId:int}/vehicles/{vehicleId:int}/services/{serviceId:int}/parts")]
public class AppointmentPartsController(AppointmentAppService appointmentAppService) : ControllerBase
{
    #region Parts

    /// <summary>
    /// Lists parts for a specific vehicle within an appointment.
    /// Requires 'ACTION_VIEW_MAIN_MENU_APPOINTMENTS' action.
    /// </summary>
    [HttpGet]
    [RequireAction("ACTION_VIEW_MAIN_MENU_APPOINTMENTS")]
    [ProducesResponseType(typeof(IEnumerable<AppointmentPartDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetParts(int appointmentId, int vehicleId, int serviceId, CancellationToken ct)
    {
        var parts = await appointmentAppService.GetPartsAsync(appointmentId, vehicleId, serviceId, ct);
        return Ok(parts);
    }

    /// <summary>
    /// Adds a part to a specific vehicle within an appointment.
    /// Requires 'ACTION_EDIT_APPOINTMENT' action.
    /// </summary>
    [HttpPost]
    [RequireAction("ACTION_EDIT_APPOINTMENT")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddPart(int appointmentId, int vehicleId, int serviceId, [FromBody] AppointmentPartCreateDto dto, CancellationToken ct)
    {
        await appointmentAppService.AddPartAsync(appointmentId, vehicleId, serviceId, dto, ct);
        return Created();
    }

    /// <summary>
    /// Updates an existing part in a vehicle.
    /// Requires 'ACTION_EDIT_APPOINTMENT' action.
    /// </summary>
    [HttpPut("{partId:int}")]
    [RequireAction("ACTION_EDIT_APPOINTMENT")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePart(int appointmentId, int vehicleId, int serviceId, int partId, [FromBody] AppointmentPartUpdateDto dto, CancellationToken ct)
    {
        await appointmentAppService.UpdatePartAsync(appointmentId, vehicleId, serviceId, partId, dto, ct);
        return NoContent();
    }

    /// <summary>
    /// Removes a part from a vehicle within an appointment.
    /// Requires 'ACTION_EDIT_APPOINTMENT' action.
    /// </summary>
    [HttpDelete("{partId:int}")]
    [RequireAction("ACTION_EDIT_APPOINTMENT")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePart(int appointmentId, int vehicleId, int serviceId, int partId, CancellationToken ct)
    {
        await appointmentAppService.DeletePartAsync(appointmentId, vehicleId, serviceId, partId, ct);
        return NoContent();
    }

    #endregion
}
