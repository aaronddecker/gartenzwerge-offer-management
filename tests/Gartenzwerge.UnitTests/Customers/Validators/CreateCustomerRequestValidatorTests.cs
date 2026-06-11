using FluentAssertions;
using Gartenzwerge.Application.Customers.DTOs;
using Gartenzwerge.Application.Customers.Validators;

namespace Gartenzwerge.UnitTests.Customers.Validators;

public class CreateCustomerRequestValidatorTests
{
    [Fact]
    public void Validate_ShouldBeValid_WhenRequestIsValid()
    {
        // Arrange
        var validator = new CreateCustomerRequestValidator();

        var request = new CreateCustomerRequest
        {
            FirstName = "Max",
            LastName = "Mustermann",
            Email = "max.mustermann@example.com",
            PhoneNumber = "0123456789",
            Company = "Musterfirma",
            Street = "Hauptstraﬂe",
            HouseNumber = "12",
            PostalCode = "12345",
            City = "Musterstadt",
            Notes = "Test customer"
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenFirstNameIsEmpty()
    {
        // Arrange
        var validator = new CreateCustomerRequestValidator();

        var request = new CreateCustomerRequest
        {
            FirstName = "",
            LastName = "Mustermann"
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenLastNameIsEmpty()
    {
        // Arrange
        var validator = new CreateCustomerRequestValidator();

        var request = new CreateCustomerRequest
        {
            FirstName = "Max",
            LastName = ""
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenEmailIsInvalid()
    {
        // Arrange
        var validator = new CreateCustomerRequestValidator();

        var request = new CreateCustomerRequest
        {
            FirstName = "Max",
            LastName = "Mustermann",
            Email = "not-an-email"
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenCompanyIsTooLong()
    {
        // Arrange
        var validator = new CreateCustomerRequestValidator();

        var request = new CreateCustomerRequest
        {
            FirstName = "Max",
            LastName = "Mustermann",
            Company = new string('A', 151)
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }
}