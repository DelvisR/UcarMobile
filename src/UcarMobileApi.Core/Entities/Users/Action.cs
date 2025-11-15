using System.Collections.Generic;

namespace UcarMobileApi.Core.Entities.Users;

public class Action : EntityBase
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Resource { get; set; } = "RESOURCE_DEFAULT";

    // Navigation properties
    public virtual ICollection<RoleAction> RoleActions { get; set; } = new List<RoleAction>();
}
