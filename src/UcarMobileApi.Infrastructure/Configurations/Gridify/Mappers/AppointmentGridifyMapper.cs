using System.Linq;
using Gridify;
using UcarMobileApi.Core.Entities.Appointments;

namespace UcarMobileApi.Infrastructure.Configurations.Gridify.Mappers;

public class AppointmentGridifyMapper : GridifyMapper<Appointment>
{
    public AppointmentGridifyMapper()
    {
        // Map all regular properties
        GenerateMappings();

        // Map a virtual field "Global" that combines multiple columns
        AddMap("Global", l => l.Client.FirstName + " " + l.Client.LastName);
        AddMap("StatusValue", a => (byte)a.Status);
        AddMap("ClientVehicleId", a => a.Vehicles.Select(v => v.VehicleId));
    }
}

public class AppointmentNoteGridifyMapper : GridifyMapper<AppointmentNote>
{
    public AppointmentNoteGridifyMapper()
    {
        // Map all regular properties
        GenerateMappings();
    }
}
