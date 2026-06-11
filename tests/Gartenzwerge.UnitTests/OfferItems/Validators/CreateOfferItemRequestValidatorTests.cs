using FluentAssertions;
using Gartenzwerge.Application.OfferItems.DTOs;
using Gartenzwerge.Application.OfferItems.Validators;

namespace Gartenzwerge.UnitTests.OfferItems.Validators;

public class CreateOfferItemRequestValidatorTests
{
    [Fact]
    public void Validate_ShouldBeValid_WhenRequestIsValid()
    {
        // Arrange
        var validator = new CreateOfferItemRequestValidator();

        var request = new CreateOfferItemRequest
        {
            OfferedServiceId = Guid.NewGuid(),
            Quantity = 250
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenOfferedServiceIdIsEmpty()
    {
        // Arrange
        var validator = new CreateOfferItemRequestValidator();

        var request = new CreateOfferItemRequest
        {
            OfferedServiceId = Guid.Empty,
            Quantity = 250
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenQuantityIsZeroOrNegative()
    {
        // Arrange
        var validator = new CreateOfferItemRequestValidator();

        var request = new CreateOfferItemRequest
        {
            OfferedServiceId = Guid.NewGuid(),
            Quantity = 0
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }
}