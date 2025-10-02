using RpgQuestManager.Api.DTOs.Auth;

namespace RpgQuestManager.Api.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}

