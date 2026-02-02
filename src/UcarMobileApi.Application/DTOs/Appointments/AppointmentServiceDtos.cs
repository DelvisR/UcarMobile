using System.Collections.Generic;

namespace UcarMobileApi.Application.DTOs.Appointments;

// Represents a specific service requested for the vehicle
public abstract class AppointmentServiceUpsertDto
{
    public int? ServiceCategoryId { get; set; }
    public int? ServiceId { get; set; }
    public string? CustomService { get; set; }
    public decimal Price { get; set; }
}

public class AppointmentServiceCreateDto : AppointmentServiceUpsertDto
{
    public ICollection<AppointmentPartCreateDto> Parts { get; set; } = [];
}

public class AppointmentServiceUpdateDto : AppointmentServiceUpsertDto;

public class AppointmentServiceDto
{
    public int Id { get; set; }
    public int? ServiceId { get; set; }
    public string ServiceCategory { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public bool IsWarrantyApplicable { get; set; } = false;
    public decimal Price { get; set; }

    public List<AppointmentPartDto> Parts { get; set; } = [];
}
