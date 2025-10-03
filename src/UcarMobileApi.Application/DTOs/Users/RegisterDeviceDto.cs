using UcarMobileApi.Core.Enums;

namespace UcarMobileApi.Application.DTOs.Users;

public class RegisterDeviceDto
{
    public int UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DevicePlatform Platform { get; set; }
}
