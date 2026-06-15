using FluentAssertions;
using Gartenzwerge.Application.Auth.DTOs;
using Gartenzwerge.Application.Auth.Validators;

namespace Gartenzwerge.UnitTests.Auth.Validators;

/// <summary>
/// This class contains unit tests for the RegisterRequestValidator, which validates the input for user registration requests.
/// </summary>
public class RegisterRequestValidatorTests
{
    private readonly RegisterRequestValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_ShouldBeValid()
    {
        var request = new RegisterRequest
        {
            Email = "test@gartenzwerge.de",
            DisplayName = "Test User",
            Password = "Test1234"
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyEmail_ShouldBeInvalid()
    {
        var request = new RegisterRequest
        {
            Email = "",
            DisplayName = "Test User",
            Password = "Test1234"
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithInvalidEmail_ShouldBeInvalid()
    {
        var request = new RegisterRequest
        {
            Email = "not-an-email",
            DisplayName = "Test User",
            Password = "Test1234"
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithEmptyDisplayName_ShouldBeInvalid()
    {
        var request = new RegisterRequest
        {
            Email = "test@gartenzwerge.de",
            DisplayName = "",
            Password = "Test1234"
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithTooShortPassword_ShouldBeInvalid()
    {
        var request = new RegisterRequest
        {
            Email = "test@gartenzwerge.de",
            DisplayName = "Test User",
            Password = "abc"
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
    }
}