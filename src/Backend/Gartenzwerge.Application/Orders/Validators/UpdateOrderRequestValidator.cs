using FluentValidation;
using Gartenzwerge.Application.Orders.DTOs;

namespace Gartenzwerge.Application.Orders.Validators;

/// <summary>
/// This class provides validation rules for the UpdateOrderRequest, ensuring that the status is a valid enum value, the planned date is in the future if provided, and the notes do not exceed a certain length.
/// </summary>
public class UpdateOrderRequestValidator : AbstractValidator<UpdateOrderRequest>
{
    public UpdateOrderRequestValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum();

        RuleFor(x => x.PlannedDate)
            .Must(plannedDate => plannedDate is null || plannedDate > DateTime.UtcNow)
            .WithMessage("The planned date must be in the future.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000);
    }
}