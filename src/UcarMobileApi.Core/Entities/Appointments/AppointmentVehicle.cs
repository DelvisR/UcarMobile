using System.Collections.Generic;
using UcarMobileApi.Core.Entities.Clients;
using UcarMobileApi.Core.Entities.Technicians;

namespace UcarMobileApi.Core.Entities.Appointments;

public class AppointmentVehicle : EntityBase
{
    public int AppointmentId { get; set; }
    public Appointment Appointment { get; set; } = null!;

    public int TechnicianId { get; set; }
    public Technician Technician { get; set; } = null!;

    public int VehicleId { get; set; }
    public ClientVehicle Vehicle { get; set; } = null!;

    public ICollection<AppointmentService> Services { get; set; } = [];
    public ICollection<AppointmentPart> Parts { get; set; } = [];
}
