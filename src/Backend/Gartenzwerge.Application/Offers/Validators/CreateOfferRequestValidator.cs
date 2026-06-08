using FluentValidation;
using Gartenzwerge.Application.Offers.DTOs;

namespace Gartenzwerge.Application.Offers.Validators;

/// <summary>
/// Validates incoming requests for creating offers.
/// </summary>
public class CreateOfferRequestValidator : AbstractValidator<CreateOfferRequest>
{
    public CreateOfferRequestValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty();

        RuleFor(x => x.ValidUntil)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("The offer validity date must be in the future.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000);
    }
}