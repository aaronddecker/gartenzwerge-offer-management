using FluentValidation;
using Gartenzwerge.Application.Auth.DTOs;

namespace Gartenzwerge.Application.Auth.Validators;

/// <summary>
/// This class defines validation rules for the RegisterRequest DTO using FluentValidation.
/// </summary>
public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.DisplayName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8);
    }
}