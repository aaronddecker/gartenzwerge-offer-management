using FluentAssertions;
using Gartenzwerge.Application.Common.Exceptions;
using Gartenzwerge.Application.OfferItems.DTOs;
using Gartenzwerge.Application.OfferItems.Services;
using Gartenzwerge.Domain.Entities;
using Gartenzwerge.UnitTests.OfferedServices;
using Gartenzwerge.UnitTests.Offers;

namespace Gartenzwerge.UnitTests.OfferItems;

/// <summary>
/// Contains unit tests for the OfferItemService class.
/// 
/// Verifies that adding an offer item works correctly when the offer and offered service exist.
/// </summary>
public class OfferItemServiceTests
{
    // Test: Adding an offer item to an existing offer with an existing offered service should succeed
    [Fact]
    public async Task AddItemAsync_ShouldAddOfferItem_WhenOfferAndOfferedServiceExist()
    {
        // Arrange
        var offerRepository = new FakeOfferRepository();
        var offeredServiceRepository = new FakeOfferedServiceRepository();

        var offer = await offerRepository.AddAsync(new Offer
        {
            CustomerId = Guid.NewGuid(),
            OfferNumber = "O-TEST-001",
            ValidUntil = DateTime.UtcNow.AddDays(14)
        });

        var offeredService = await offeredServiceRepository.AddAsync(new OfferedService
        {
            Name = "Rasen mähen",
            Description = "Mowing lawn areas based on square meters.",
            Unit = "m²",
            BasePrice = 0.18m,
            IsActive = true
        });

        var service = new OfferItemService(
            offerRepository,
            offeredServiceRepository);

        var request = new CreateOfferItemRequest
        {
            OfferedServiceId = offeredService.Id,
            Quantity = 250
        };

        // Act
        var result = await service.AddItemAsync(offer.Id, request);

        // Assert
        result.Should().NotBeNull();
        result!.OfferId.Should().Be(offer.Id);
        result.OfferedServiceId.Should().Be(offeredService.Id);
        result.Description.Should().Be("Rasen mähen");
        result.Quantity.Should().Be(250);
        result.Unit.Should().Be("m²");
        result.UnitPrice.Should().Be(0.18m);
        result.TotalPrice.Should().Be(45.00m);
        offer.TotalNet.Should().Be(45.00m);
    }

    // Test: Adding an offer item to a non-existent offer should throw a NotFoundException
    [Fact]
    public async Task AddItemAsync_ShouldThrowNotFoundException_WhenOfferDoesNotExist()
    {
        // Arrange
        var offerRepository = new FakeOfferRepository();
        var offeredServiceRepository = new FakeOfferedServiceRepository();

        var offeredService = await offeredServiceRepository.AddAsync(new OfferedService
        {
            Name = "Rasen mähen",
            Description = "Mowing lawn areas based on square meters.",
            Unit = "m²",
            BasePrice = 0.18m,
            IsActive = true
        });

        var service = new OfferItemService(
            offerRepository,
            offeredServiceRepository);

        var request = new CreateOfferItemRequest
        {
            OfferedServiceId = offeredService.Id,
            Quantity = 250
        };

        // Act
        var act = async () => await service.AddItemAsync(Guid.NewGuid(), request);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("Offer was not found.");
    }

    // Test: Adding an offer item with a non-existent offered service should throw a NotFoundException
    [Fact]
    public async Task AddItemAsync_ShouldThrowNotFoundException_WhenOfferedServiceDoesNotExist()
    {
        // Arrange
        var offerRepository = new FakeOfferRepository();
        var offeredServiceRepository = new FakeOfferedServiceRepository();

        var offer = await offerRepository.AddAsync(new Offer
        {
            CustomerId = Guid.NewGuid(),
            OfferNumber = "O-TEST-001",
            ValidUntil = DateTime.UtcNow.AddDays(14)
        });

        var service = new OfferItemService(
            offerRepository,
            offeredServiceRepository);

        var request = new CreateOfferItemRequest
        {
            OfferedServiceId = Guid.NewGuid(),
            Quantity = 250
        };

        // Act
        var act = async () => await service.AddItemAsync(offer.Id, request);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("Offered service was not found.");
    }
}