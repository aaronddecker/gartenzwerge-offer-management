using FluentValidation;
using Gartenzwerge.Application.Orders.DTOs;

namespace Gartenzwerge.Application.Orders.Validators;

/// <summary>
/// this class provides validation rules for the CreateOrderFromOfferRequest, ensuring that the planned date is in the future and that notes do not exceed a certain length.
/// </summary>
public class CreateOrderFromOfferRequestValidator : AbstractValidator<CreateOrderFromOfferRequest>
{
    public CreateOrderFromOfferRequestValidator()
    {
        RuleFor(x => x.PlannedDate)
            .Must(plannedDate => plannedDate is null || plannedDate > DateTime.UtcNow)
            .WithMessage("The planned date must be in the future.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000);
    }
}