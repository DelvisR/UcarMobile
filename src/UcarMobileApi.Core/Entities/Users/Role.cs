using System.Collections.Generic;

namespace UcarMobileApi.Core.Entities.Users;

public class Role : EntityBase
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    // Navigation properties
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public virtual ICollection<RoleAction> RoleActions { get; set; } = new List<RoleAction>();
}
