using FluentAssertions;
using Gartenzwerge.Application.Orders.DTOs;
using Gartenzwerge.Application.Orders.Services;
using Gartenzwerge.Application.Common.Exceptions;
using Gartenzwerge.Domain.Entities;
using Gartenzwerge.Domain.Enums;
using Gartenzwerge.UnitTests.Offers;


namespace Gartenzwerge.UnitTests.Orders;

/// <summary>
/// This class contains unit tests for the OrderService class, specifically testing the CreateFromOfferAsync method.
/// </summary>
public class OrderServiceTests
{
    [Fact]
    public async Task CreateFromOfferAsync_ShouldCreateOrder_WhenOfferIsAccepted()
    {
        // Arrange
        var orderRepository = new FakeOrderRepository();
        var offerRepository = new FakeOfferRepository();

        var offer = await offerRepository.AddAsync(new Offer
        {
            CustomerId = Guid.NewGuid(),
            OfferNumber = "O-TEST-001",
            ValidUntil = DateTime.UtcNow.AddDays(14),
            Status = OfferStatus.Accepted,
            TotalNet = 45.00m
        });

        var service = new OrderService(
            orderRepository,
            offerRepository);

        var request = new CreateOrderFromOfferRequest
        {
            PlannedDate = DateTime.UtcNow.AddDays(7),
            Notes = "Test order from accepted offer."
        };

        // Act
        var result = await service.CreateFromOfferAsync(offer.Id, request);

        // Assert
        result.Id.Should().NotBeEmpty();
        result.OfferId.Should().Be(offer.Id);
        result.CustomerId.Should().Be(offer.CustomerId);
        result.Status.Should().Be(OrderStatus.Planned);
        result.PlannedDate.Should().Be(request.PlannedDate);
        result.CompletedAt.Should().BeNull();
        result.Notes.Should().Be(request.Notes);
    }
    [Fact]
    public async Task CreateFromOfferAsync_ShouldThrowNotFoundException_WhenOfferDoesNotExist()
    {
        // Arrange
        var orderRepository = new FakeOrderRepository();
        var offerRepository = new FakeOfferRepository();

        var service = new OrderService(
            orderRepository,
            offerRepository);

        var request = new CreateOrderFromOfferRequest();

        // Act
        var act = async () => await service.CreateFromOfferAsync(
            Guid.NewGuid(),
            request);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("Offer was not found.");
    }

    [Fact]
    public async Task CreateFromOfferAsync_ShouldThrowConflictException_WhenOfferIsNotAccepted()
    {
        // Arrange
        var orderRepository = new FakeOrderRepository();
        var offerRepository = new FakeOfferRepository();

        var offer = await offerRepository.AddAsync(new Offer
        {
            CustomerId = Guid.NewGuid(),
            OfferNumber = "O-TEST-001",
            ValidUntil = DateTime.UtcNow.AddDays(14),
            Status = OfferStatus.Draft,
            TotalNet = 45.00m
        });

        var service = new OrderService(
            orderRepository,
            offerRepository);

        var request = new CreateOrderFromOfferRequest();

        // Act
        var act = async () => await service.CreateFromOfferAsync(
            offer.Id,
            request);

        // Assert
        await act.Should()
            .ThrowAsync<ConflictException>()
            .WithMessage("Only accepted offers can be converted into orders.");
    }

    [Fact]
    public async Task CreateFromOfferAsync_ShouldThrowConflictException_WhenOrderAlreadyExists()
    {
        // Arrange
        var orderRepository = new FakeOrderRepository();
        var offerRepository = new FakeOfferRepository();

        var offer = await offerRepository.AddAsync(new Offer
        {
            CustomerId = Guid.NewGuid(),
            OfferNumber = "O-TEST-001",
            ValidUntil = DateTime.UtcNow.AddDays(14),
            Status = OfferStatus.Accepted,
            TotalNet = 45.00m
        });

        await orderRepository.AddAsync(new Order
        {
            OfferId = offer.Id,
            CustomerId = offer.CustomerId,
            Status = OrderStatus.Planned
        });

        var service = new OrderService(
            orderRepository,
            offerRepository);

        var request = new CreateOrderFromOfferRequest();

        // Act
        var act = async () => await service.CreateFromOfferAsync(
            offer.Id,
            request);

        // Assert
        await act.Should()
            .ThrowAsync<ConflictException>()
            .WithMessage("An order already exists for this offer.");
    }
}