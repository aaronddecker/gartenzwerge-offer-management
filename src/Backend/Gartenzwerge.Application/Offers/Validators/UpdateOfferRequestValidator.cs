using FluentValidation;
using Gartenzwerge.Application.Offers.DTOs;

namespace Gartenzwerge.Application.Offers.Validators;

/// <summary>
/// Validates incoming requests for updating offers.
/// </summary>
public class UpdateOfferRequestValidator : AbstractValidator<UpdateOfferRequest>
{
    public UpdateOfferRequestValidator()
    {
        RuleFor(x => x.ValidUntil)
            .Must(validUntil => validUntil > DateTime.UtcNow)
            .WithMessage("The offer validity date must be in the future.");

        RuleFor(x => x.Status)
            .IsInEnum();

        RuleFor(x => x.Notes)
            .MaximumLength(1000);
    }
}