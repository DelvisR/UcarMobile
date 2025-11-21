using UcarMobileApi.Core.Entities.Services;

namespace UcarMobileApi.Core.Entities.Appointments;

public class AppointmentService : EntityBase
{
    public int AppointmentId { get; set; }
    public Appointment Appointment { get; set; } = null!;

    public int ServiceId { get; set; }
    public Service Service { get; set; } = null!;

    public decimal Price { get; set; }

    public string? Notes { get; set; }
}
