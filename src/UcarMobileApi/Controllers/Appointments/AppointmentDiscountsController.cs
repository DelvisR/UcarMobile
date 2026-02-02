using Microsoft.AspNetCore.Mvc;
using UcarMobileApi.Application.DTOs.Appointments;
using UcarMobileApi.Application.Services.Appointments;
using UcarMobileApi.Authorization;
using UcarMobileApi.Core.Entities.Appointments;

namespace UcarMobileApi.Controllers.Appointments;

/// <summary>
/// API controller for managing discounts applied to an appointment.
/// </summary>
[Route("api/appointments/{appointmentId:int}/discounts")]
public class AppointmentDiscountsController(AppointmentAppService appointmentAppService) : ControllerBase
{
    #region Discounts

    /// <summary>
    /// Applies a discount to an appointment.
    /// Requires 'ACTION_EDIT_APPOINTMENT'.
    /// </summary>
    /// <param name="appointmentId">The appointment ID.</param>
    /// <param name="dto">Discount details.</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpPost]
    [RequireAction("ACTION_EDIT_APPOINTMENT")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApplyDiscount(int appointmentId, [FromBody] ApplyDiscountDto dto, CancellationToken ct)
    {
        await appointmentAppService.ApplyDiscountAsync(appointmentId, dto, ct);
        return NoContent();
    }

    /// <summary>
    /// Applies the review discount to an appointment (e.g., for leaving a review).
    /// Requires 'ACTION_EDIT_APPOINTMENT'.
    /// </summary>
    /// <param name="appointmentId">The appointment ID.</param>
    /// <param name="source">The discount source.</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpPost("review")]
    [RequireAction("ACTION_EDIT_APPOINTMENT")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApplyReviewDiscount(int appointmentId, [FromQuery] UcarMobileApi.Core.Entities.Appointments.DiscountSource source, CancellationToken ct)
    {
        await appointmentAppService.ApplyReviewDiscountAsync(appointmentId, source, ct);
        return NoContent();
    }

    /// <summary>
    /// Removes a discount from an appointment by category.
    /// Requires 'ACTION_EDIT_APPOINTMENT'.
    /// </summary>
    /// <param name="appointmentId">The appointment ID.</param>
    /// <param name="category">The discount category to remove.</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpDelete]
    [RequireAction("ACTION_EDIT_APPOINTMENT")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveDiscount(int appointmentId, [FromQuery] DiscountCategory category, CancellationToken ct)
    {
        await appointmentAppService.RemoveDiscountAsync(appointmentId, category, ct);
        return NoContent();
    }

    #endregion
}
