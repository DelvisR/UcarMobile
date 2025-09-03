using System.Collections.Generic;

namespace UcarMobileApi.Core.Entities.Users;

public class Permission : EntityBase
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    // Navigation properties
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}