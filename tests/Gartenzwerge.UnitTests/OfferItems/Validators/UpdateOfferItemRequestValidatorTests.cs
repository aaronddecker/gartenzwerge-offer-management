using FluentAssertions;
using Gartenzwerge.Application.OfferItems.DTOs;
using Gartenzwerge.Application.OfferItems.Validators;

namespace Gartenzwerge.UnitTests.OfferItems.Validators;

public class UpdateOfferItemRequestValidatorTests
{
    [Fact]
    public void Validate_ShouldBeValid_WhenRequestIsValid()
    {
        // Arrange
        var validator = new UpdateOfferItemRequestValidator();

        var request = new UpdateOfferItemRequest
        {
            Quantity = 300
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenQuantityIsZeroOrNegative()
    {
        // Arrange
        var validator = new UpdateOfferItemRequestValidator();

        var request = new UpdateOfferItemRequest
        {
            Quantity = 0
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }
}