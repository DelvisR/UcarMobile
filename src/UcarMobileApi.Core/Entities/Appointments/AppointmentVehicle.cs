using System.Collections.Generic;
using UcarMobileApi.Core.Entities.Clients;
using UcarMobileApi.Core.Entities.Common;
using UcarMobileApi.Core.Entities.Technicians;

namespace UcarMobileApi.Core.Entities.Appointments;

public class AppointmentVehicle : EntityBase
{
    public int AppointmentId { get; set; }
    public Appointment Appointment { get; set; } = null!;

    public int? TechnicianId { get; set; }
    public Technician? Technician { get; set; }

    public int VehicleId { get; set; }
    public ClientVehicle Vehicle { get; set; } = null!;

    public int OdometerKm { get; set; }

    public ICollection<AppointmentService> Services { get; set; } = [];
}
