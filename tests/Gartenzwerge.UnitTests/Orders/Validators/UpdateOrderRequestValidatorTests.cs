using FluentAssertions;
using Gartenzwerge.Application.Orders.DTOs;
using Gartenzwerge.Application.Orders.Validators;
using Gartenzwerge.Domain.Enums;

namespace Gartenzwerge.UnitTests.Orders.Validators;

public class UpdateOrderRequestValidatorTests
{
    [Fact]
    public void Validate_ShouldBeValid_WhenRequestIsValid()
    {
        // Arrange
        var validator = new UpdateOrderRequestValidator();

        var request = new UpdateOrderRequest
        {
            Status = OrderStatus.InProgress,
            PlannedDate = DateTime.UtcNow.AddDays(7),
            Notes = "Order is now in progress."
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
        var validator = new UpdateOrderRequestValidator();

        var request = new UpdateOrderRequest
        {
            Status = OrderStatus.Planned,
            PlannedDate = null,
            Notes = "Order without planned date."
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenStatusIsInvalid()
    {
        // Arrange
        var validator = new UpdateOrderRequestValidator();

        var request = new UpdateOrderRequest
        {
            Status = (OrderStatus)999,
            PlannedDate = DateTime.UtcNow.AddDays(7),
            Notes = "Invalid order status."
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenPlannedDateIsInThePast()
    {
        // Arrange
        var validator = new UpdateOrderRequestValidator();

        var request = new UpdateOrderRequest
        {
            Status = OrderStatus.Planned,
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
        var validator = new UpdateOrderRequestValidator();

        var request = new UpdateOrderRequest
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