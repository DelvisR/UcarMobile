using System.Collections.Generic;

namespace UcarMobileApi.Core.Entities.Services;

public class Service : EntityBase
{
    public string Name { get; set; } = string.Empty;

    public int ServiceCategoryId { get; set; }
    public ServiceCategory ServiceCategory { get; set; } = null!;

    public ICollection<Estimate> Estimates { get; set; } = [];

}
