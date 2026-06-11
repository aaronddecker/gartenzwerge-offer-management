using Gartenzwerge.Application.Orders.Interfaces;
using Gartenzwerge.Domain.Entities;
using Gartenzwerge.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gartenzwerge.Infrastructure.Repositories;

/// <summary>
/// Provides data access operations for orders using Entity Framework Core.
/// </summary>
public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _dbContext;

    public OrderRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Order> AddAsync(Order order)
    {
        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        return order;
    }

    public async Task<Order?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Orders
            .Include(x => x.Offer)
            .Include(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Order?> GetByOfferIdAsync(Guid offerId)
    {
        return await _dbContext.Orders
            .FirstOrDefaultAsync(x => x.OfferId == offerId);
    }

    public async Task<IReadOnlyList<Order>> GetAllAsync()
    {
        return await _dbContext.Orders
            .AsNoTracking()
            .Include(x => x.Offer)
            .Include(x => x.Customer)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task UpdateAsync(Order order)
    {
        order.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }
}