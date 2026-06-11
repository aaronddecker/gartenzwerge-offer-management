using Gartenzwerge.Application.Orders.DTOs;

namespace Gartenzwerge.Application.Orders.Interfaces;

/// <summary>
/// Defines operations for creating and managing orders.
/// </summary>
public interface IOrderService
{
    Task<OrderDto> CreateFromOfferAsync(Guid offerId, CreateOrderFromOfferRequest request);
}