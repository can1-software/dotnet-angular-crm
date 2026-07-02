using Crm.Application.DTOs.Auth;

namespace Crm.Application.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(RegisterRequest request);
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request);
    Task LogoutAsync(LogoutRequest request);
    Task<CurrentUserResponse> GetCurrentUserAsync(Guid userId);
}
