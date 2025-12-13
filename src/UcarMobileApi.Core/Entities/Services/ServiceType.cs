using System.Collections.Generic;
using UcarMobileApi.Core.Entities.Common;

namespace UcarMobileApi.Core.Entities.Services;

public class ServiceType : EntityBase
{
    public string Title { get; set; } = string.Empty;

    public ICollection<ServiceCategory> Categories { get; set; } = [];
}
