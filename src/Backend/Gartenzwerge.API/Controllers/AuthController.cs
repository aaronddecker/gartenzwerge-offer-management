using Gartenzwerge.Application.Auth.DTOs;
using Gartenzwerge.Application.Auth.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Gartenzwerge.API.Controllers;

/// <summary>
/// This controller handles user authentication, including registration and login.
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        var response = await _authService.RegisterAsync(request);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        return Ok(response);
    }

    [Authorize]
    [HttpGet("me")]
    public ActionResult<object> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = User.FindFirstValue(ClaimTypes.Email);
        var displayName = User.FindFirstValue(ClaimTypes.Name);

        return Ok(new
        {
            userId,
            email,
            displayName
        });
    }
}