using Microsoft.AspNetCore.Mvc;
using UcarMobileApi.Application.Common.Models;
using UcarMobileApi.Application.DTOs.Appointments;
using UcarMobileApi.Application.Services.Appointments;
using UcarMobileApi.Web.Extensions;

namespace UcarMobileApi.Controllers.Appointments;

[ApiController]
[Route("api/appointments")]
public class AppointmentsController(AppointmentService appointmentService) : ControllerBase
{
    /// <summary>
    /// Gets all appointments with filtering, sorting, and pagination.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointments([FromQuery] QueryFilter query, CancellationToken ct)
    {
        var (headers, dtos) = await appointmentService.GetAppointmentsAsync(query, ct);
        return Ok(dtos).WithHeaders(headers);
    }

    /// <summary>
    /// Creates a new appointment.
    /// </summary>
    /// <param name="dto">The appointment creation data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created appointment.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAppointment([FromBody] AppointmentCreateDto dto, CancellationToken ct)
    {
        var appointment = await appointmentService.CreateAppointmentAsync(dto, ct);
        return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
    }

    /// <summary>
    /// Retrieves an appointment by its ID.
    /// </summary>
    /// <param name="id">The appointment ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The appointment details.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAppointment(int id, CancellationToken ct)
    {
        var appointment = await appointmentService.GetAppointmentAsync(id, ct);
        return Ok(appointment);
    }

    /// <summary>
    /// Retrieves all appointments for a specific client.
    /// </summary>
    /// <param name="clientId">The client ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of appointments.</returns>
    [HttpGet("client/{clientId:int}")]
    [ProducesResponseType(typeof(List<AppointmentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetClientAppointments(int clientId, CancellationToken ct)
    {
        var appointments = await appointmentService.GetClientAppointmentsAsync(clientId, ct);
        return Ok(appointments);
    }
}
