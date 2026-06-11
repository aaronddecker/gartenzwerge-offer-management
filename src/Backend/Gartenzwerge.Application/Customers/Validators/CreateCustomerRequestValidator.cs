using FluentValidation;
using Gartenzwerge.Application.Customers.DTOs;

namespace Gartenzwerge.Application.Customers.Validators;

/// <summary>
/// Validates incoming requests for creating customers.
///
/// Validation rules are kept in the Application layer because they are part
/// of the application's use-case boundaries and protect the domain from invalid input.
/// </summary>
public class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerRequestValidator()
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

        RuleFor(x => x.Company)
            .MaximumLength(150);

        RuleFor(x => x.Street)
            .MaximumLength(150);

        RuleFor(x => x.HouseNumber)
            .MaximumLength(20);
    }
}