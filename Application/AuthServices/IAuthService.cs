using Application.Models;

namespace Application.AuthServices;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterModel model, CancellationToken cancellationToken = default);
    Task<AuthResponse> LoginAsync(LoginModel model, CancellationToken cancellationToken = default);
}
