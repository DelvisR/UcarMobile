using UcarMobileApi.Core.Entities.Common;

namespace UcarMobileApi.Core.Entities.Appointments;

public class AppointmentPart : EntityBase
{
    public int AppointmentServiceId { get; set; }
    public AppointmentService AppointmentService { get; set; } = null!;

    public string PartName { get; set; } = string.Empty;
    public string? PartNumber { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }

    // Indicates whether the installed or used part was supplied by the customer 
    // rather than provided by the service center.
    public bool IsCustomerProvidedPart { get; set; } = false;

    public string? Note { get; set; }
}
