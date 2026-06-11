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

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllOrders()
    {
        // Arrange
        var orderRepository = new FakeOrderRepository();
        var offerRepository = new FakeOfferRepository();

        var firstOrder = await orderRepository.AddAsync(new Order
        {
            OfferId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            Status = OrderStatus.Planned,
            PlannedDate = DateTime.UtcNow.AddDays(7),
            Notes = "First test order"
        });

        var secondOrder = await orderRepository.AddAsync(new Order
        {
            OfferId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            Status = OrderStatus.InProgress,
            PlannedDate = DateTime.UtcNow.AddDays(14),
            Notes = "Second test order"
        });

        var service = new OrderService(
            orderRepository,
            offerRepository);

        // Act
        var result = await service.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);

        result.Should().Contain(x => x.Id == firstOrder.Id);
        result.Should().Contain(x => x.Id == secondOrder.Id);

        result.Should().Contain(x => x.Status == OrderStatus.Planned);
        result.Should().Contain(x => x.Status == OrderStatus.InProgress);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnOrder_WhenOrderExists()
    {
        // Arrange
        var orderRepository = new FakeOrderRepository();
        var offerRepository = new FakeOfferRepository();

        var order = await orderRepository.AddAsync(new Order
        {
            OfferId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            Status = OrderStatus.Planned,
            PlannedDate = DateTime.UtcNow.AddDays(7),
            Notes = "Test order"
        });

        var service = new OrderService(
            orderRepository,
            offerRepository);

        // Act
        var result = await service.GetByIdAsync(order.Id);

        // Assert
        result.Id.Should().Be(order.Id);
        result.OfferId.Should().Be(order.OfferId);
        result.CustomerId.Should().Be(order.CustomerId);
        result.Status.Should().Be(order.Status);
        result.PlannedDate.Should().Be(order.PlannedDate);
        result.Notes.Should().Be(order.Notes);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowNotFoundException_WhenOrderDoesNotExist()
    {
        // Arrange
        var orderRepository = new FakeOrderRepository();
        var offerRepository = new FakeOfferRepository();

        var service = new OrderService(
            orderRepository,
            offerRepository);

        // Act
        var act = async () => await service.GetByIdAsync(Guid.NewGuid());

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("Order was not found.");
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateOrder_WhenOrderExists()
    {
        // Arrange
        var orderRepository = new FakeOrderRepository();
        var offerRepository = new FakeOfferRepository();

        var order = await orderRepository.AddAsync(new Order
        {
            OfferId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            Status = OrderStatus.Planned,
            PlannedDate = DateTime.UtcNow.AddDays(7),
            Notes = "Initial notes"
        });

        var service = new OrderService(
            orderRepository,
            offerRepository);

        var request = new UpdateOrderRequest
        {
            Status = OrderStatus.InProgress,
            PlannedDate = DateTime.UtcNow.AddDays(14),
            Notes = "Updated notes"
        };

        // Act
        var result = await service.UpdateAsync(order.Id, request);

        // Assert
        result.Id.Should().Be(order.Id);
        result.Status.Should().Be(OrderStatus.InProgress);
        result.PlannedDate.Should().Be(request.PlannedDate);
        result.Notes.Should().Be("Updated notes");
        result.CompletedAt.Should().BeNull();

        order.Status.Should().Be(OrderStatus.InProgress);
        order.PlannedDate.Should().Be(request.PlannedDate);
        order.Notes.Should().Be("Updated notes");
    }

    [Fact]
    public async Task UpdateAsync_ShouldSetCompletedAt_WhenStatusIsCompleted()
    {
        // Arrange
        var orderRepository = new FakeOrderRepository();
        var offerRepository = new FakeOfferRepository();

        var order = await orderRepository.AddAsync(new Order
        {
            OfferId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            Status = OrderStatus.InProgress,
            PlannedDate = DateTime.UtcNow.AddDays(7),
            Notes = "Order in progress"
        });

        var service = new OrderService(
            orderRepository,
            offerRepository);

        var request = new UpdateOrderRequest
        {
            Status = OrderStatus.Completed,
            PlannedDate = order.PlannedDate,
            Notes = "Order completed"
        };

        // Act
        var result = await service.UpdateAsync(order.Id, request);

        // Assert
        result.Status.Should().Be(OrderStatus.Completed);
        result.CompletedAt.Should().NotBeNull();

        order.Status.Should().Be(OrderStatus.Completed);
        order.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_ShouldClearCompletedAt_WhenStatusIsNotCompleted()
    {
        // Arrange
        var orderRepository = new FakeOrderRepository();
        var offerRepository = new FakeOfferRepository();

        var order = await orderRepository.AddAsync(new Order
        {
            OfferId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            Status = OrderStatus.Completed,
            CompletedAt = DateTime.UtcNow.AddDays(-1),
            PlannedDate = DateTime.UtcNow.AddDays(7),
            Notes = "Completed order"
        });

        var service = new OrderService(
            orderRepository,
            offerRepository);

        var request = new UpdateOrderRequest
        {
            Status = OrderStatus.InProgress,
            PlannedDate = order.PlannedDate,
            Notes = "Order reopened"
        };

        // Act
        var result = await service.UpdateAsync(order.Id, request);

        // Assert
        result.Status.Should().Be(OrderStatus.InProgress);
        result.CompletedAt.Should().BeNull();

        order.Status.Should().Be(OrderStatus.InProgress);
        order.CompletedAt.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowNotFoundException_WhenOrderDoesNotExist()
    {
        // Arrange
        var orderRepository = new FakeOrderRepository();
        var offerRepository = new FakeOfferRepository();

        var service = new OrderService(
            orderRepository,
            offerRepository);

        var request = new UpdateOrderRequest
        {
            Status = OrderStatus.InProgress,
            PlannedDate = DateTime.UtcNow.AddDays(7),
            Notes = "Trying to update a non-existing order"
        };

        // Act
        var act = async () => await service.UpdateAsync(
            Guid.NewGuid(),
            request);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("Order was not found.");
    }
}