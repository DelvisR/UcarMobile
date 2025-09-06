using System.Threading;
using System.Threading.Tasks;
using UcarMobileApi.Application.DTOs.Users;

namespace UcarMobileApi.Application.Services.Auth;

public interface IAuthService
{
    Task RegisterUserAsync(UserDto userDto, CancellationToken ct);
    Task<object> GetCurrentUserAsync(string cognitoId, CancellationToken ct);
}