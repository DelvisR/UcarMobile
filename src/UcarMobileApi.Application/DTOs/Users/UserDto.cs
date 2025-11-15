using System.Collections.Generic;

namespace UcarMobileApi.Application.DTOs.Users;

public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string? LangKey { get; set; }
    public string? AuthProviderId { get; set; }
    public bool IsActive { get; set; }
}

public class UserAccountDto : UserDto
{
    public List<RoleDto>? Roles { get; set; }
}

public class RoleNameDto
{
    public string Name { get; set; } = string.Empty;
}

public class CurrentUserDto : UserDto
{
    public List<RoleNameDto>? Roles { get; set; }
}

/// <summary>
/// Represents a action assigned to a user including its resource.
/// </summary>
public record UserActionDto(string Name, string Resource);
