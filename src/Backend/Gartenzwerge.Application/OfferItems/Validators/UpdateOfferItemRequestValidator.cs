using FluentValidation;
using Gartenzwerge.Application.OfferItems.DTOs;

namespace Gartenzwerge.Application.OfferItems.Validators;

public class UpdateOfferItemRequestValidator : AbstractValidator<UpdateOfferItemRequest>
{
    public UpdateOfferItemRequestValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0);
    }
}