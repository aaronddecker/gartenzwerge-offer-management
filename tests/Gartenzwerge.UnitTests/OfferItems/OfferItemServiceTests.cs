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
/// Verifies adding, retrieving and updating offer items,
/// including price calculation, total recalculation and not-found handling.
/// </summary>
public class OfferItemServiceTests
{
    [Fact]
    public async Task AddItemAsync_ShouldAddOfferItem_WhenOfferAndOfferedServiceExist()
    {
        // Arrange
        var context = CreateTestContext();

        var offer = await CreateOfferAsync(context.OfferRepository);
        var offeredService = await CreateOfferedServiceAsync(context.OfferedServiceRepository);

        var request = new CreateOfferItemRequest
        {
            OfferedServiceId = offeredService.Id,
            Quantity = 250
        };

        // Act
        var result = await context.Service.AddItemAsync(offer.Id, request);

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

    [Fact]
    public async Task AddItemAsync_ShouldThrowNotFoundException_WhenOfferDoesNotExist()
    {
        // Arrange
        var context = CreateTestContext();

        var offeredService = await CreateOfferedServiceAsync(context.OfferedServiceRepository);

        var request = new CreateOfferItemRequest
        {
            OfferedServiceId = offeredService.Id,
            Quantity = 250
        };

        // Act
        var act = async () => await context.Service.AddItemAsync(Guid.NewGuid(), request);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("Offer was not found.");
    }

    [Fact]
    public async Task AddItemAsync_ShouldThrowNotFoundException_WhenOfferedServiceDoesNotExist()
    {
        // Arrange
        var context = CreateTestContext();

        var offer = await CreateOfferAsync(context.OfferRepository);

        var request = new CreateOfferItemRequest
        {
            OfferedServiceId = Guid.NewGuid(),
            Quantity = 250
        };

        // Act
        var act = async () => await context.Service.AddItemAsync(offer.Id, request);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("Offered service was not found.");
    }

    [Fact]
    public async Task GetItemsByOfferIdAsync_ShouldReturnItems_WhenOfferExists()
    {
        // Arrange
        var context = CreateTestContext();

        var offer = await CreateOfferAsync(context.OfferRepository);
        var offeredService = await CreateOfferedServiceAsync(context.OfferedServiceRepository);

        await context.Service.AddItemAsync(offer.Id, new CreateOfferItemRequest
        {
            OfferedServiceId = offeredService.Id,
            Quantity = 250
        });

        // Act
        var result = await context.Service.GetItemsByOfferIdAsync(offer.Id);

        // Assert
        result.Should().HaveCount(1);
        result[0].Description.Should().Be("Rasen mähen");
        result[0].Quantity.Should().Be(250);
        result[0].Unit.Should().Be("m²");
        result[0].UnitPrice.Should().Be(0.18m);
        result[0].TotalPrice.Should().Be(45.00m);
    }

    [Fact]
    public async Task UpdateItemAsync_ShouldUpdateQuantityAndTotalPrice_WhenOfferItemExists()
    {
        // Arrange
        var context = CreateTestContext();

        var offer = await CreateOfferAsync(context.OfferRepository);
        var offeredService = await CreateOfferedServiceAsync(context.OfferedServiceRepository);

        var createdItem = await context.Service.AddItemAsync(offer.Id, new CreateOfferItemRequest
        {
            OfferedServiceId = offeredService.Id,
            Quantity = 250
        });

        createdItem.Should().NotBeNull();

        // Act
        var updatedItem = await context.Service.UpdateItemAsync(
            offer.Id,
            createdItem!.Id,
            new UpdateOfferItemRequest
            {
                Quantity = 300
            });

        // Assert
        updatedItem.Quantity.Should().Be(300);
        updatedItem.TotalPrice.Should().Be(54.00m);
        offer.TotalNet.Should().Be(54.00m);
    }

    [Fact]
    public async Task UpdateItemAsync_ShouldThrowNotFoundException_WhenOfferDoesNotExist()
    {
        // Arrange
        var context = CreateTestContext();

        var request = new UpdateOfferItemRequest
        {
            Quantity = 300
        };

        // Act
        var act = async () => await context.Service.UpdateItemAsync(
            Guid.NewGuid(),
            Guid.NewGuid(),
            request);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("Offer was not found.");
    }

    [Fact]
    public async Task UpdateItemAsync_ShouldThrowNotFoundException_WhenOfferItemDoesNotExist()
    {
        // Arrange
        var context = CreateTestContext();
        var offer = await CreateOfferAsync(context.OfferRepository);

        var request = new UpdateOfferItemRequest
        {
            Quantity = 300
        };

        // Act
        var act = async () => await context.Service.UpdateItemAsync(
            offer.Id,
            Guid.NewGuid(),
            request);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("Offer item was not found.");
    }

    [Fact]
    public async Task DeleteItemAsync_ShouldSoftDeleteOfferItemAndRecalculateTotalNet_WhenOfferItemExists()
    {
        // Arrange
        var context = CreateTestContext();

        var offer = await CreateOfferAsync(context.OfferRepository);
        var offeredService = await CreateOfferedServiceAsync(context.OfferedServiceRepository);

        var createdItem = await context.Service.AddItemAsync(offer.Id, new CreateOfferItemRequest
        {
            OfferedServiceId = offeredService.Id,
            Quantity = 250
        });

        createdItem.Should().NotBeNull();

        // Act
        await context.Service.DeleteItemAsync(offer.Id, createdItem!.Id);

        // Assert
        offer.Items.Should().ContainSingle();

        var deletedItem = offer.Items.Single();

        deletedItem.IsDeleted.Should().BeTrue();
        deletedItem.DeletedAt.Should().NotBeNull();
        offer.TotalNet.Should().Be(0);
    }

    [Fact]
    public async Task DeleteItemAsync_ShouldThrowNotFoundException_WhenOfferDoesNotExist()
    {
        // Arrange
        var context = CreateTestContext();

        // Act
        var act = async () => await context.Service.DeleteItemAsync(
            Guid.NewGuid(),
            Guid.NewGuid());

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("Offer was not found.");
    }

    [Fact]
    public async Task DeleteItemAsync_ShouldThrowNotFoundException_WhenOfferItemDoesNotExist()
    {
        // Arrange
        var context = CreateTestContext();

        var offer = await CreateOfferAsync(context.OfferRepository);

        // Act
        var act = async () => await context.Service.DeleteItemAsync(
            offer.Id,
            Guid.NewGuid());

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("Offer item was not found.");
    }

    private static Task<Offer> CreateOfferAsync(FakeOfferRepository repository)
    {
        return repository.AddAsync(new Offer
        {
            CustomerId = Guid.NewGuid(),
            OfferNumber = "O-TEST-001",
            ValidUntil = DateTime.UtcNow.AddDays(14)
        });
    }

    private static Task<OfferedService> CreateOfferedServiceAsync(
        FakeOfferedServiceRepository repository)
    {
        return repository.AddAsync(new OfferedService
        {
            Name = "Rasen mähen",
            Description = "Mowing lawn areas based on square meters.",
            Unit = "m²",
            BasePrice = 0.18m,
            IsActive = true
        });
    }

    private static (
        OfferItemService Service,
        FakeOfferRepository OfferRepository,
        FakeOfferedServiceRepository OfferedServiceRepository)
        CreateTestContext()
    {
        var offerRepository = new FakeOfferRepository();
        var offeredServiceRepository = new FakeOfferedServiceRepository();

        var service = new OfferItemService(
            offerRepository,
            offeredServiceRepository);

        return (
            service,
            offerRepository,
            offeredServiceRepository);
    }
}