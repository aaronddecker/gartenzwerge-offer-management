using Gartenzwerge.Application.Common.Exceptions;
using Gartenzwerge.Application.Offers.Interfaces;
using Gartenzwerge.Application.Orders.DTOs;
using Gartenzwerge.Application.Orders.Interfaces;
using Gartenzwerge.Domain.Entities;
using Gartenzwerge.Domain.Enums;

namespace Gartenzwerge.Application.Orders.Services;
/// <summary>
/// This service implements application-level use cases for order management, specifically creating orders from accepted offers.
/// </summary>
public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOfferRepository _offerRepository;

    public OrderService(
        IOrderRepository orderRepository,
        IOfferRepository offerRepository)
    {
        _orderRepository = orderRepository;
        _offerRepository = offerRepository;
    }

    public async Task<OrderDto> CreateFromOfferAsync(
        Guid offerId,
        CreateOrderFromOfferRequest request)
    {
        var offer = await _offerRepository.GetByIdAsync(offerId);

        if (offer is null)
        {
            throw new NotFoundException("Offer was not found.");
        }

        if (offer.Status != OfferStatus.Accepted)
        {
            throw new ConflictException("Only accepted offers can be converted into orders.");
        }

        var existingOrder = await _orderRepository.GetByOfferIdAsync(offerId);

        if (existingOrder is not null)
        {
            throw new ConflictException("An order already exists for this offer.");
        }

        var order = new Order
        {
            OfferId = offer.Id,
            CustomerId = offer.CustomerId,
            Status = OrderStatus.Planned,
            PlannedDate = request.PlannedDate,
            Notes = request.Notes
        };

        var createdOrder = await _orderRepository.AddAsync(order);

        return MapToDto(createdOrder);
    }

    public async Task<IReadOnlyList<OrderDto>> GetAllAsync()
    {
        var orders = await _orderRepository.GetAllAsync();

        return orders
            .Select(MapToDto)
            .ToList();
    }

    public async Task<OrderDto> GetByIdAsync(Guid id)
    {
        var order = await _orderRepository.GetByIdAsync(id);

        if (order is null)
        {
            throw new NotFoundException("Order was not found.");
        }

        return MapToDto(order);
    }

    public async Task<OrderDto> UpdateAsync(Guid id, UpdateOrderRequest request)
    {
        var order = await _orderRepository.GetByIdAsync(id);

        if (order is null)
        {
            throw new NotFoundException("Order was not found.");
        }

        order.Status = request.Status;
        order.PlannedDate = request.PlannedDate;
        order.Notes = request.Notes;

        if (request.Status == OrderStatus.Completed && order.CompletedAt is null)
        {
            order.CompletedAt = DateTime.UtcNow;
        }

        if (request.Status != OrderStatus.Completed)
        {
            order.CompletedAt = null;
        }

        await _orderRepository.UpdateAsync(order);

        return MapToDto(order);
    }

    private static OrderDto MapToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            OfferId = order.OfferId,
            CustomerId = order.CustomerId,
            Status = order.Status,
            PlannedDate = order.PlannedDate,
            CompletedAt = order.CompletedAt,
            Notes = order.Notes
        };
    }

    public async Task DeleteAsync(Guid id)
    {
        var order = await _orderRepository.GetByIdAsync(id);

        if (order is null)
        {
            throw new NotFoundException("Order was not found.");
        }

        order.IsDeleted = true;
        order.DeletedAt = DateTime.UtcNow;

        await _orderRepository.UpdateAsync(order);
    }
}