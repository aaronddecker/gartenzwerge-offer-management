namespace Gartenzwerge.Application.Auth.DTOs;

/// <summary>
/// This class represents the data transfer object for user registration requests. It contains the necessary information for creating a new user account, such as email, display name, and password.
/// </summary>
public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}