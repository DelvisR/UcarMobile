namespace UcarMobileApi.Application.DTOs.Appointments;

// Represents a part required for the service
public class AppointmentPartCreateDto
{
    public string PartName { get; set; } = string.Empty;
    public string? PartNumber { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? Notes { get; set; }
}

public class AppointmentPartDto
{
    public int Id { get; set; }

    public string PartName { get; set; } = string.Empty;
    public string? PartNumber { get; set; }

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public string? Notes { get; set; }
}
