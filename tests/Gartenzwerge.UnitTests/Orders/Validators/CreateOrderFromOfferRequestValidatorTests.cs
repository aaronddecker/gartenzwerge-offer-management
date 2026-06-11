using FluentAssertions;
using Gartenzwerge.Application.Orders.DTOs;
using Gartenzwerge.Application.Orders.Validators;

namespace Gartenzwerge.UnitTests.Orders.Validators;

public class CreateOrderFromOfferRequestValidatorTests
{
    [Fact]
    public void Validate_ShouldBeValid_WhenRequestIsValid()
    {
        // Arrange
        var validator = new CreateOrderFromOfferRequestValidator();

        var request = new CreateOrderFromOfferRequest
        {
            PlannedDate = DateTime.UtcNow.AddDays(7),
            Notes = "Planned garden work."
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldBeValid_WhenPlannedDateIsNull()
    {
        // Arrange
        var validator = new CreateOrderFromOfferRequestValidator();

        var request = new CreateOrderFromOfferRequest
        {
            PlannedDate = null,
            Notes = "Order without planned date."
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenPlannedDateIsInThePast()
    {
        // Arrange
        var validator = new CreateOrderFromOfferRequestValidator();

        var request = new CreateOrderFromOfferRequest
        {
            PlannedDate = DateTime.UtcNow.AddDays(-1),
            Notes = "Invalid planned date."
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenNotesAreTooLong()
    {
        // Arrange
        var validator = new CreateOrderFromOfferRequestValidator();

        var request = new CreateOrderFromOfferRequest
        {
            PlannedDate = DateTime.UtcNow.AddDays(7),
            Notes = new string('A', 1001)
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }
}