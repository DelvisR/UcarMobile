using System.Collections.Generic;

namespace UcarMobileApi.Core.Entities.Users;

public class User : EntityBase
{
    public string CognitoId { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}";

    public string Email { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}