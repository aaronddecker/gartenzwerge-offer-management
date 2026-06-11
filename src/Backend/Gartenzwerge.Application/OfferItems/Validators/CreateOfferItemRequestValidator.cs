using FluentValidation;
using Gartenzwerge.Application.OfferItems.DTOs;

namespace Gartenzwerge.Application.OfferItems.Validators;

public class CreateOfferItemRequestValidator : AbstractValidator<CreateOfferItemRequest>
{
    public CreateOfferItemRequestValidator()
    {
        RuleFor(x => x.OfferedServiceId)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .GreaterThan(0);
    }
}