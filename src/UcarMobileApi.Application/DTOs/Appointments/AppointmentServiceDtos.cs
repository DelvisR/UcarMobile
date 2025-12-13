namespace UcarMobileApi.Application.DTOs.Appointments;

// Represents a specific service requested for the vehicle
public class AppointmentServiceCreateDto
{
    public int ServiceId { get; set; }
    public decimal Price { get; set; }
    public string? Notes { get; set; }
}

public class AppointmentServiceDto
{
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;

    public decimal Price { get; set; }
    public string? Notes { get; set; }
}
