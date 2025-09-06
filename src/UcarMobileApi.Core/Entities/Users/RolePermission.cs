namespace UcarMobileApi.Core.Entities.Users;

public class RolePermission : EntityBase
{
    public int RoleId { get; set; }
    public int PermissionId { get; set; }

    // Navigation properties
    public virtual Role Role { get; set; } = null!;
    public virtual Permission Permission { get; set; } = null!;
}