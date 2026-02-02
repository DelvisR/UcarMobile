namespace UcarMobileApi.Application.Validators.Users;

public static class UserValidationConstants
{
    public const long MaxImageSize = 2 * 1024 * 1024;
    public static readonly string[] AllowedMimeTypes = ["image/jpeg", "image/png", "image/webp"];
}
