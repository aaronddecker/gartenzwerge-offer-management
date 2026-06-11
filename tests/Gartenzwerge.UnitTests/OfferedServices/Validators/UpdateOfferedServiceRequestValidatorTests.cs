using FluentAssertions;
using Gartenzwerge.Application.OfferedServices.DTOs;
using Gartenzwerge.Application.OfferedServices.Validators;

namespace Gartenzwerge.UnitTests.OfferedServices.Validators;

public class UpdateOfferedServiceRequestValidatorTests
{
    [Fact]
    public void Validate_ShouldBeValid_WhenRequestIsValid()
    {
        // Arrange
        var validator = new UpdateOfferedServiceRequestValidator();

        var request = new UpdateOfferedServiceRequest
        {
            Name = "Rasen mähen",
            Description = "Mowing lawn areas based on square meters.",
            Unit = "m²",
            BasePrice = 0.18m,
            IsActive = true
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenNameIsEmpty()
    {
        // Arrange
        var validator = new UpdateOfferedServiceRequestValidator();

        var request = new UpdateOfferedServiceRequest
        {
            Name = "",
            Unit = "m²",
            BasePrice = 0.18m
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenUnitIsEmpty()
    {
        // Arrange
        var validator = new UpdateOfferedServiceRequestValidator();

        var request = new UpdateOfferedServiceRequest
        {
            Name = "Rasen mähen",
            Unit = "",
            BasePrice = 0.18m
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenBasePriceIsNegative()
    {
        // Arrange
        var validator = new UpdateOfferedServiceRequestValidator();

        var request = new UpdateOfferedServiceRequest
        {
            Name = "Rasen mähen",
            Unit = "m²",
            BasePrice = -1
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenDescriptionIsTooLong()
    {
        // Arrange
        var validator = new UpdateOfferedServiceRequestValidator();

        var request = new UpdateOfferedServiceRequest
        {
            Name = "Rasen mähen",
            Description = new string('A', 1001),
            Unit = "m²",
            BasePrice = 0.18m
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }
}