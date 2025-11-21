using UcarMobileApi.Core.Entities.Clients;

namespace UcarMobileApi.Core.Entities.Appointments;

public class AppointmentVehicle : EntityBase
{
    public int AppointmentId { get; set; }
    public Appointment Appointment { get; set; } = null!;

    public int TechnicianId { get; set; }
    public Technicians.Technician Technician { get; set; } = null!;

    public int VehicleId { get; set; }
    public ClientVehicle Vehicle { get; set; } = null!;
}
