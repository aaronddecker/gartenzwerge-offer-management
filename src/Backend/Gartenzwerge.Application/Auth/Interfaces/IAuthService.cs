using Gartenzwerge.Application.Auth.DTOs;

namespace Gartenzwerge.Application.Auth.Interfaces;

/// <summary>
/// This interface defines the contract for the authentication service, which handles user registration and login operations.
/// </summary>
public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}