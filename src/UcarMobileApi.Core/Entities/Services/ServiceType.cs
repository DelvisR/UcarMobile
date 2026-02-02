using System.Collections.Generic;
using UcarMobileApi.Core.Entities.Common;

namespace UcarMobileApi.Core.Entities.Services;

public class ServiceType : EntityBase
{
    public string Title { get; set; } = string.Empty;

    // Indicates whether the global warranty (12 months / 12,000 miles) applies to this service type.
    // Services such as towing, glass repair, or labor-only jobs with customer-provided parts are excluded.
    public bool IsWarrantyApplicable { get; set; } = false;

    public ICollection<ServiceCategory> Categories { get; set; } = [];
}
