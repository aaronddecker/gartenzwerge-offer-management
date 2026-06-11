using FluentAssertions;
using Gartenzwerge.Application.Offers.DTOs;
using Gartenzwerge.Domain.Enums;
using Gartenzwerge.Application.Offers.Validators;

namespace Gartenzwerge.UnitTests.Offers.Validators;

public class UpdateOfferRequestValidatorTests
{
    [Fact]
    public void Validate_ShouldBeValid_WhenRequestIsValid()
    {
        // Arrange
        var validator = new UpdateOfferRequestValidator();

        var request = new UpdateOfferRequest
        {
            ValidUntil = DateTime.UtcNow.AddDays(14),
            Status = OfferStatus.Sent,
            Notes = "Valid offer notes"
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenValidUntilIsInThePast()
    {
        // Arrange
        var validator = new UpdateOfferRequestValidator();

        var request = new UpdateOfferRequest
        {
            ValidUntil = DateTime.UtcNow.AddDays(-1),
            Status = OfferStatus.Sent
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenStatusIsInvalid()
    {
        // Arrange
        var validator = new UpdateOfferRequestValidator();

        var request = new UpdateOfferRequest
        {
            ValidUntil = DateTime.UtcNow.AddDays(14),
            Status = (OfferStatus)999
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
        var validator = new UpdateOfferRequestValidator();

        var request = new UpdateOfferRequest
        {
            ValidUntil = DateTime.UtcNow.AddDays(14),
            Status = OfferStatus.Sent,
            Notes = new string('A', 1001)
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }
}