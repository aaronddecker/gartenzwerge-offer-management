using Gartenzwerge.Application.Auth.DTOs;
using Gartenzwerge.Application.Auth.Interfaces;
using Gartenzwerge.Application.Common.Exceptions;
using Gartenzwerge.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Gartenzwerge.Infrastructure.Authentication;

/// <summary>
/// This service implements the authentication logic for user registration and login using ASP.NET Core Identity.
/// </summary>
public class AuthService : IAuthService
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly IConfiguration _configuration;

	public AuthService(
		UserManager<ApplicationUser> userManager,
		IConfiguration configuration)
	{
		_userManager = userManager;
		_configuration = configuration;
	}

	public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
	{
		var existingUser = await _userManager.FindByEmailAsync(request.Email);

		if (existingUser is not null)
		{
			throw new ConflictException("A user with this email already exists.");
		}

		var user = new ApplicationUser
		{
			UserName = request.Email,
			Email = request.Email,
			DisplayName = request.DisplayName
		};

		var result = await _userManager.CreateAsync(user, request.Password);

		if (!result.Succeeded)
		{
			var errors = string.Join(" ", result.Errors.Select(error => error.Description));
			throw new ConflictException(errors);
		}

		return new AuthResponse
		{
			UserId = user.Id,
			Email = user.Email ?? string.Empty,
			DisplayName = user.DisplayName,
			Token = GenerateJwtToken(user)
		};
	}

	public async Task<AuthResponse> LoginAsync(LoginRequest request)
	{
		var user = await _userManager.FindByEmailAsync(request.Email);

		if (user is null)
		{
			throw new UnauthorizedException("Invalid email or password.");
		}

		var passwordIsValid = await _userManager.CheckPasswordAsync(user, request.Password);

		if (!passwordIsValid)
		{
			throw new UnauthorizedException("Invalid email or password.");
		}

		return new AuthResponse
		{
			UserId = user.Id,
			Email = user.Email ?? string.Empty,
			DisplayName = user.DisplayName,
			Token = GenerateJwtToken(user)
		};
	}

	private string GenerateJwtToken(ApplicationUser user)
	{
		var issuer = _configuration["Jwt:Issuer"];
		var audience = _configuration["Jwt:Audience"];
		var secret = _configuration["Jwt:Secret"];
		var expiresInMinutes = int.Parse(_configuration["Jwt:ExpiresInMinutes"] ?? "60");

		if (string.IsNullOrWhiteSpace(secret))
		{
			throw new InvalidOperationException("JWT secret is not configured.");
		}

		var claims = new List<Claim>
		{
			new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
			new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
			new(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new(ClaimTypes.Name, user.DisplayName),
			new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
		};

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
		var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var token = new JwtSecurityToken(
			issuer: issuer,
			audience: audience,
			claims: claims,
			expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
			signingCredentials: credentials);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}
}