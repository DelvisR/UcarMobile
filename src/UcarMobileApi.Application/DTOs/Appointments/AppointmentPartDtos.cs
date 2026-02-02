namespace UcarMobileApi.Application.DTOs.Appointments;

// Represents a part required for the service
public abstract class AppointmentPartUpsertDto
{
    public string PartName { get; set; } = string.Empty;
    public string? PartNumber { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public bool IsCustomerProvidedPart { get; set; } = false;
    public string? Note { get; set; }
}

public class AppointmentPartCreateDto : AppointmentPartUpsertDto;
public class AppointmentPartUpdateDto : AppointmentPartUpsertDto;

public class AppointmentPartDto
{
    public int Id { get; set; }
    public string PartName { get; set; } = string.Empty;
    public string? PartNumber { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public bool IsCustomerProvidedPart { get; set; } = false;
    public string? Note { get; set; }
}
