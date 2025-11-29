using System.Collections.Generic;

namespace UcarMobileApi.Core.Entities.Users;

public abstract class UserBase : EntityBase
{
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? LangKey { get; set; }
    public bool IsActive { get; set; } = false;
    public string? AuthProviderId { get; set; } // Cognito, Auth0, etc.

    // Navigation properties
    public virtual ICollection<UserRole> UserRoles { get; set; } = [];
}

public class User : UserBase { }
