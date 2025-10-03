namespace UcarMobileApi.Core.Entities.Users;

public class RoleAction : EntityBase
{
    public int RoleId { get; set; }
    public int ActionId { get; set; }

    // Navigation properties
    public virtual Role Role { get; set; } = null!;
    public virtual Action Action { get; set; } = null!;
}
