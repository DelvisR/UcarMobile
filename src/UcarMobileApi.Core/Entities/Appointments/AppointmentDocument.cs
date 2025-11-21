using UcarMobileApi.Core.Entities.Storage;

namespace UcarMobileApi.Core.Entities.Appointments;

public class AppointmentDocument : EntityBase
{
    public int AppointmentId { get; set; }
    public Appointment Appointment { get; set; } = null!;
    public int StoredFileId { get; set; }
    public StoredFile StoredFile { get; set; } = null!;
    public int? AppointmentNoteId { get; set; } // The document may belong to a note
    public AppointmentNote? AppointmentNote { get; set; }
}
