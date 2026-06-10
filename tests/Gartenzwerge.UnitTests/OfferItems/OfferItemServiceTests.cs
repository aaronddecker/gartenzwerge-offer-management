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

    // Test: Retrieving offer items by offer ID should return the correct items when the offer exists
    [Fact]
    public async Task GetItemsByOfferIdAsync_ShouldReturnItems_WhenOfferExists()
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

        await service.AddItemAsync(offer.Id, new CreateOfferItemRequest
        {
            OfferedServiceId = offeredService.Id,
            Quantity = 250
        });

        // Act
        var result = await service.GetItemsByOfferIdAsync(offer.Id);

        // Assert
        result.Should().HaveCount(1);
        result[0].Description.Should().Be("Rasen mähen");
        result[0].Quantity.Should().Be(250);
        result[0].Unit.Should().Be("m²");
        result[0].UnitPrice.Should().Be(0.18m);
        result[0].TotalPrice.Should().Be(45.00m);
    }

    // Test: Updating an existing offer item should correctly update the quantity and total price
    [Fact]
    public async Task UpdateItemAsync_ShouldUpdateQuantityAndTotalPrice_WhenOfferItemExists()
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

        var createdItem = await service.AddItemAsync(offer.Id, new CreateOfferItemRequest
        {
            OfferedServiceId = offeredService.Id,
            Quantity = 250
        });

        // Act
        var updatedItem = await service.UpdateItemAsync(
            offer.Id,
            createdItem.Id,
            new UpdateOfferItemRequest
            {
                Quantity = 300
            });

        // Assert
        updatedItem.Quantity.Should().Be(300);
        updatedItem.TotalPrice.Should().Be(54.00m);
        offer.TotalNet.Should().Be(54.00m);
    }

    // Test: Updating an offer item for a non-existent offer should throw a NotFoundException
    [Fact]
    public async Task UpdateItemAsync_ShouldThrowNotFoundException_WhenOfferDoesNotExist()
    {
        // Arrange
        var offerRepository = new FakeOfferRepository();
        var offeredServiceRepository = new FakeOfferedServiceRepository();

        var service = new OfferItemService(
            offerRepository,
            offeredServiceRepository);

        var request = new UpdateOfferItemRequest
        {
            Quantity = 300
        };

        // Act
        var act = async () => await service.UpdateItemAsync(
            Guid.NewGuid(),
            Guid.NewGuid(),
            request);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("Offer was not found.");
    }

    // Test: Updating a non-existent offer item should throw a NotFoundException
    [Fact]
    public async Task UpdateItemAsync_ShouldThrowNotFoundException_WhenOfferItemDoesNotExist()
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

        var request = new UpdateOfferItemRequest
        {
            Quantity = 300
        };

        // Act
        var act = async () => await service.UpdateItemAsync(
            offer.Id,
            Guid.NewGuid(),
            request);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("Offer item was not found.");
    }
}