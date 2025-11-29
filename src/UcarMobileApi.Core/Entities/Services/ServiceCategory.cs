using System.Collections.Generic;
using UcarMobileApi.Core.Entities.Technicians;

namespace UcarMobileApi.Core.Entities.Services;

public class ServiceCategory : EntityBase
{
    public string Name { get; set; } = string.Empty;

    public int ServiceTypeId { get; set; }
    public ServiceType ServiceType { get; set; } = null!;

    public ICollection<Service> Services { get; set; } = [];

    public ICollection<TechnicianSpeciality> TechnicianSpecialities { get; set; } = [];

}
