using FluentValidation;
using Gartenzwerge.Application.Auth.DTOs;

namespace Gartenzwerge.Application.Auth.Validators;

/// <summary>
/// This class defines validation rules for the LoginRequest DTO using FluentValidation.
/// </summary>
public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}