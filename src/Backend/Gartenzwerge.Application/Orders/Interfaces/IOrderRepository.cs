using Gartenzwerge.Domain.Entities;

namespace Gartenzwerge.Application.Orders.Interfaces;

/// <summary>
/// This interface defines the contract for data access operations related to orders.
/// </summary>
public interface IOrderRepository
{
    Task<Order> AddAsync(Order order);

    Task<Order?> GetByIdAsync(Guid id);

    Task<Order?> GetByOfferIdAsync(Guid offerId);

    Task<IReadOnlyList<Order>> GetAllAsync();

    Task UpdateAsync(Order order);
}