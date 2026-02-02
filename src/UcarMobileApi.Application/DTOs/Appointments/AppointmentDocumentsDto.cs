using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using UcarMobileApi.Application.DTOs.Files;
using UcarMobileApi.Core.Entities.Appointments;

namespace UcarMobileApi.Application.DTOs.Appointments;

public abstract class DocumentDtoBase
{
    public StoredFileDto File { get; set; } = null!;
}

public sealed class AppointmentDocumentDto : DocumentDtoBase
{
    public ContentSource Source { get; set; } = ContentSource.Technician;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}

public class AddAppointmentDocumentsDto
{
    public List<IFormFile> Files { get; set; } = [];

    public string? Prefix { get; set; }

    public ContentSource Source { get; set; }
}

