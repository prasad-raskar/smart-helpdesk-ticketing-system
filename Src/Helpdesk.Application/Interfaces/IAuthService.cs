using Helpdesk.Application.DTOs;

namespace Helpdesk.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegisterAsync(CreateUserRequest request);
}
