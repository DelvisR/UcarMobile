using System.Collections.Generic;
using UcarMobileApi.Core.Entities.Common;
using UcarMobileApi.Core.Entities.Storage;

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

    public int? ImageStoredFileId { get; set; } // Image, avatar, etc.
    public StoredFile? ImageStoredFile { get; set; }

    // Navigation properties
    public virtual ICollection<UserRole> UserRoles { get; set; } = [];
}

public class User : UserBase
{
    public ICollection<Device> Devices { get; set; } = [];
}
