using System.Collections.Generic;
using UcarMobileApi.Core.Entities.Common;
using UcarMobileApi.Core.Entities.Services;

namespace UcarMobileApi.Core.Entities.Appointments;

public class AppointmentService : EntityBase
{
    public int AppointmentVehicleId { get; set; }
    public AppointmentVehicle AppointmentVehicle { get; set; } = null!;

    public int? ServiceId { get; set; }
    public Service? Service { get; set; }

    public string? CustomService { get; set; }

    public string? ServiceName => Service?.Name ?? CustomService;

    public decimal Price { get; set; }

    public ICollection<AppointmentPart> Parts { get; set; } = [];
}
