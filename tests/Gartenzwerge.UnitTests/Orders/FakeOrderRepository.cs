using Gartenzwerge.Application.Orders.Interfaces;
using Gartenzwerge.Domain.Entities;

namespace Gartenzwerge.UnitTests.Orders;

/// <summary>
/// Tis class provides a fake implementation of the IOrderRepository interface for unit testing purposes.
/// </summary>
public class FakeOrderRepository : IOrderRepository
{
    private readonly List<Order> _orders = new();

    public Task<Order> AddAsync(Order order)
    {
        _orders.Add(order);
        return Task.FromResult(order);
    }

    public Task<Order?> GetByIdAsync(Guid id)
    {
        return Task.FromResult(_orders.FirstOrDefault(x => x.Id == id && !x.IsDeleted));
    }

    public Task<Order?> GetByOfferIdAsync(Guid offerId)
    {
        return Task.FromResult(_orders.FirstOrDefault(x => x.OfferId == offerId && !x.IsDeleted));
    }

    public Task<IReadOnlyList<Order>> GetAllAsync()
    {
        return Task.FromResult<IReadOnlyList<Order>>(
            _orders.Where(x => !x.IsDeleted).ToList());
    }

    public Task UpdateAsync(Order order)
    {
        order.UpdatedAt = DateTime.UtcNow;
        return Task.CompletedTask;
    }
}