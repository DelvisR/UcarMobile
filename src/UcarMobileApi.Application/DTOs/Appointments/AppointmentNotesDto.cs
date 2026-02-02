using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using UcarMobileApi.Core.Entities.Appointments;

namespace UcarMobileApi.Application.DTOs.Appointments;

public class AppointmentNoteBaseDto
{
    public string Content { get; set; } = string.Empty;

    public ContentSource Source { get; set; } = ContentSource.Client;
}

public class AppointmentNoteCreateDto : AppointmentNoteBaseDto
{
    public IEnumerable<IFormFile>? Files { get; set; }
    public string? Prefix { get; set; }
}

public sealed class AppointmentNoteDocumentDto : DocumentDtoBase { }

public class AppointmentNoteDto : AppointmentNoteBaseDto
{
    public int Id { get; set; }

    public IReadOnlyCollection<AppointmentNoteDocumentDto> Documents { get; set; } = [];

    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
