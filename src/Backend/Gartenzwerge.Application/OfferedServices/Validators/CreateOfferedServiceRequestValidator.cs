using FluentValidation;
using Gartenzwerge.Application.OfferedServices.DTOs;

namespace Gartenzwerge.Application.OfferedServices.Validators;

/// <summary>
/// Validates incoming requests for creating offered services.
/// </summary>
public class CreateOfferedServiceRequestValidator : AbstractValidator<CreateOfferedServiceRequest>
{
    public CreateOfferedServiceRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Description)
            .MaximumLength(1000);

        RuleFor(x => x.Unit)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.BasePrice)
            .GreaterThanOrEqualTo(0);
    }
}