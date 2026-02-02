using System.Collections.Generic;
using UcarMobileApi.Application.DTOs.Files;

namespace UcarMobileApi.Application.DTOs.Users;

public class UserBaseDto
{
    public int Id { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? LangKey { get; set; }
}

public class UserDto : UserBaseDto
{
    public string FullName => $"{FirstName} {LastName}";
    public string? AuthProviderId { get; set; }

    public StoredFileDto? File { get; set; }
    public bool IsActive { get; set; }
}

public class UserUpdateDto : UserBaseDto;

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
