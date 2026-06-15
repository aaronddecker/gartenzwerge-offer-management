using FluentAssertions;
using Gartenzwerge.Application.Auth.DTOs;
using Gartenzwerge.Application.Auth.Validators;

namespace Gartenzwerge.UnitTests.Auth.Validators;

/// <summary>
/// This class contains unit tests for the LoginRequestValidator, which validates the input for user login requests.
/// </summary>
public class LoginRequestValidatorTests
{
    private readonly LoginRequestValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_ShouldBeValid()
    {
        var request = new LoginRequest
        {
            Email = "test@gartenzwerge.de",
            Password = "Test1234"
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyEmail_ShouldBeInvalid()
    {
        var request = new LoginRequest
        {
            Email = "",
            Password = "Test1234"
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithInvalidEmail_ShouldBeInvalid()
    {
        var request = new LoginRequest
        {
            Email = "not-an-email",
            Password = "Test1234"
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithEmptyPassword_ShouldBeInvalid()
    {
        var request = new LoginRequest
        {
            Email = "test@gartenzwerge.de",
            Password = ""
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
    }
}