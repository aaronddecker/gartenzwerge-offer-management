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

        return new OrderDto
        {
            Id = createdOrder.Id,
            OfferId = createdOrder.OfferId,
            CustomerId = createdOrder.CustomerId,
            Status = createdOrder.Status,
            PlannedDate = createdOrder.PlannedDate,
            CompletedAt = createdOrder.CompletedAt,
            Notes = createdOrder.Notes
        };
    }
}