using FluentValidation;
using Gartenzwerge.Application.Customers.DTOs;

namespace Gartenzwerge.Application.Customers.Validators;

/// <summary>
/// Validates incoming requests for updating customers.
/// 
/// The rules are intentionally similar to customer creation because both
/// operations require a valid customer state.
/// </summary>
public class UpdateCustomerRequestValidator : AbstractValidator<UpdateCustomerRequest>
{
    public UpdateCustomerRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(50);

        RuleFor(x => x.PostalCode)
            .MaximumLength(20);

        RuleFor(x => x.City)
            .MaximumLength(100);

        RuleFor(x => x.Notes)
            .MaximumLength(1000);
    }
}