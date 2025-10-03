using UcarMobileApi.Core.Enums;

namespace UcarMobileApi.Core.Entities.Users;

public class Device : EntityBase
{
    public int UserId { get; set; }
    public User User { get; set; } = new User();
    public string Token { get; set; } = string.Empty;
    public DevicePlatform Platform { get; set; }
    public string EndpointArn { get; set; } = string.Empty;
}
