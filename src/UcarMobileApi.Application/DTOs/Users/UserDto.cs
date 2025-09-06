using System.Collections.Generic;

namespace UcarMobileApi.Application.DTOs.Users;

public class UserDto
{
    public int Id { get; set; }
    public string CognitoId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<RoleDto> Roles { get; set; } = [];
}