using FluentAssertions;
using Gartenzwerge.Application.Offers.DTOs;
using Gartenzwerge.Application.Offers.Validators;

namespace Gartenzwerge.UnitTests.Offers.Validators;

public class CreateOfferRequestValidatorTests
{
	[Fact]
	public void Validate_ShouldBeValid_WhenRequestIsValid()
	{
		// Arrange
		var validator = new CreateOfferRequestValidator();

		var request = new CreateOfferRequest
		{
			CustomerId = Guid.NewGuid(),
			ValidUntil = DateTime.UtcNow.AddDays(14),
			Notes = "Valid offer notes"
		};

		// Act
		var result = validator.Validate(request);

		// Assert
		result.IsValid.Should().BeTrue();
	}

	[Fact]
	public void Validate_ShouldBeInvalid_WhenCustomerIdIsEmpty()
	{
		// Arrange
		var validator = new CreateOfferRequestValidator();

		var request = new CreateOfferRequest
		{
			CustomerId = Guid.Empty,
			ValidUntil = DateTime.UtcNow.AddDays(14)
		};

		// Act
		var result = validator.Validate(request);

		// Assert
		result.IsValid.Should().BeFalse();
	}

	[Fact]
	public void Validate_ShouldBeInvalid_WhenValidUntilIsInThePast()
	{
		// Arrange
		var validator = new CreateOfferRequestValidator();

		var request = new CreateOfferRequest
		{
			CustomerId = Guid.NewGuid(),
			ValidUntil = DateTime.UtcNow.AddDays(-1)
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
		var validator = new CreateOfferRequestValidator();

		var request = new CreateOfferRequest
		{
			CustomerId = Guid.NewGuid(),
			ValidUntil = DateTime.UtcNow.AddDays(14),
			Notes = new string('A', 1001)
		};

		// Act
		var result = validator.Validate(request);

		// Assert
		result.IsValid.Should().BeFalse();
	}
}