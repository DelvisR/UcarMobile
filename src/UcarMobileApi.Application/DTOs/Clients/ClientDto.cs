using UcarMobileApi.Application.DTOs.Users;

namespace UcarMobileApi.Application.DTOs.Clients;

public class ClientDto : UserDto
{
    public string Address { get; set; } = string.Empty;
}
