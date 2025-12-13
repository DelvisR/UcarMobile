using UcarMobileApi.Core.Entities.Common;
using UcarMobileApi.Core.Entities.Services;

namespace UcarMobileApi.Core.Entities.Appointments;

public class AppointmentService : AuditableEntity
{
    public int AppointmentVehicleId { get; set; }
    public AppointmentVehicle AppointmentVehicle { get; set; } = null!;

    public int ServiceId { get; set; }
    public Service Service { get; set; } = null!;

    public decimal Price { get; set; }

    public string? Notes { get; set; }
}
