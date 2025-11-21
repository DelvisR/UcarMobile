namespace UcarMobileApi.Core.Entities.Appointments;

public class AppointmentPart : EntityBase
{
    public int AppointmentId { get; set; }
    public Appointment Appointment { get; set; } = null!;

    public string PartName { get; set; } = string.Empty;
    public string? PartNumber { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Notes { get; set; }
}
