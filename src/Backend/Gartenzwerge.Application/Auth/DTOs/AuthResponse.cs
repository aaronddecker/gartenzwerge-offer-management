namespace Gartenzwerge.Application.Auth.DTOs;

/// <summary>
/// This DTO represents the response returned after a successful authentication.
/// It contains the user's unique identifier, email, display name, and the generated JWT token for subsequent authenticated requests.
/// </summary>
public class AuthResponse
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}