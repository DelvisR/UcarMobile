using UcarMobileApi.Core.Entities.Common;

namespace UcarMobileApi.Core.Entities.Users;

public class RoleAction : AuditableEntity
{
    public int RoleId { get; set; }
    public int ActionId { get; set; }

    // Navigation properties
    public virtual Role Role { get; set; } = null!;
    public virtual Action Action { get; set; } = null!;
}
