using UcarMobileApi.Core.Entities.Common;
using UcarMobileApi.Core.Entities.Storage;

namespace UcarMobileApi.Core.Entities.Appointments;

public class AppointmentDocument : AuditableEntity
{
    public int AppointmentId { get; set; }
    public Appointment Appointment { get; set; } = null!;
    public int StoredFileId { get; set; }
    public StoredFile StoredFile { get; set; } = null!;
    public ContentSource Source { get; set; } = ContentSource.Technician;
}

public class AppointmentNoteDocument : AuditableEntity
{
    public int AppointmentNoteId { get; set; }
    public AppointmentNote AppointmentNote { get; set; } = null!;

    public int StoredFileId { get; set; }
    public StoredFile StoredFile { get; set; } = null!;
}

