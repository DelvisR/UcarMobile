using System.Threading;
using System.Threading.Tasks;
using UcarMobileApi.Application.DTOs.Users;

namespace UcarMobileApi.Application.Services.Auth;

public interface IAuthService
{
    Task<UserDto> RegisterUserAsync(CreateUserDto createUsuarioDto, CancellationToken cancellationToken = default);
    Task<object> GetCurrentUserAsync(string cognitoId, CancellationToken cancellationToken = default);
    Task InitializeSystemAsync(CancellationToken cancellationToken = default);
}