using System.Collections.Generic;
using UcarMobileApi.Core.Entities.Common;

namespace UcarMobileApi.Core.Entities.Appointments;

public class AppointmentNote : EntityBase
{
    public int AppointmentId { get; set; }
    public Appointment Appointment { get; set; } = null!;

    public string Content { get; set; } = string.Empty;

    public ContentSource Source { get; set; } = ContentSource.Client;

    public ICollection<AppointmentNoteDocument> Documents { get; set; } = [];
}

public enum ContentSource : byte
{
    Client = 1,
    Technician,
    ContactCenter
}
