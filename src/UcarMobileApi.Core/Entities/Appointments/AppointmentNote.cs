namespace UcarMobileApi.Core.Entities.Appointments;

public class AppointmentNote : EntityBase
{
    public int AppointmentId { get; set; }
    public Appointment Appointment { get; set; } = null!;

    public string Content { get; set; } = string.Empty;

    public NoteSource Source { get; set; }
}

public enum NoteSource : byte
{
    Client = 1,
    Technician,
    ContactCenter
}
