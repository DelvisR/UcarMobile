using System.Collections.Generic;
using UcarMobileApi.Core.Entities.Appointments;
using UcarMobileApi.Core.Entities.Common;

namespace UcarMobileApi.Core.Entities.Services;

public class Service : EntityBase
{
    public string Name { get; set; } = string.Empty;

    public int ServiceCategoryId { get; set; }
    public ServiceCategory ServiceCategory { get; set; } = null!;

    public ICollection<AppointmentService> Appointments { get; set; } = [];
    public ICollection<Estimate> Estimates { get; set; } = [];
    public ICollection<ServicePopularity> PopularityEntries { get; set; } = [];
}
