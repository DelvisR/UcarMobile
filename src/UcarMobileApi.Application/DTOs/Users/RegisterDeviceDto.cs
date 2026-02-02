using UcarMobileApi.Core.Enums;

namespace UcarMobileApi.Application.DTOs.Users;

public class RegisterDeviceDto
{
    public string Token { get; set; } = string.Empty;
    public DevicePlatform Platform { get; set; }
}
