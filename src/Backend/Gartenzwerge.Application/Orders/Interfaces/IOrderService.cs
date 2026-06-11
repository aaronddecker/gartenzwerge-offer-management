using Gartenzwerge.Application.Orders.DTOs;

namespace Gartenzwerge.Application.Orders.Interfaces;

/// <summary>
/// This interface defines the contract for the OrderService, which provides application-level use cases for order management, specifically creating orders from accepted offers and retrieving order information.
/// </summary>
public interface IOrderService
{
    Task<OrderDto> CreateFromOfferAsync(Guid offerId, CreateOrderFromOfferRequest request);

    Task<IReadOnlyList<OrderDto>> GetAllAsync();

    Task<OrderDto> GetByIdAsync(Guid id);

    Task<OrderDto> UpdateAsync(Guid id, UpdateOrderRequest request);

    Task DeleteAsync(Guid id);
}