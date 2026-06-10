using Gartenzwerge.Application.Offers.Interfaces;
using Gartenzwerge.Domain.Entities;
using Gartenzwerge.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gartenzwerge.Infrastructure.Repositories;

/// <summary>
/// Entity Framework Core implementation of the offer repository.
/// 
/// This repository contains database-specific logic for offers
/// and keeps EF Core out of the Application layer.
/// </summary>
public class OfferRepository : IOfferRepository
{
    private readonly AppDbContext _dbContext;

    public OfferRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Offer> AddAsync(Offer offer)
    {
        _dbContext.Offers.Add(offer);

        await _dbContext.SaveChangesAsync();

        return offer;
    }

    public async Task<Offer?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Offers
            .Include(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Offer?> GetByIdWithItemsAsync(Guid id)
    {
        return await _dbContext.Offers
            .Include(x => x.Items)
                .ThenInclude(x => x.OfferedService)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IReadOnlyList<Offer>> GetAllAsync()
    {
        return await _dbContext.Offers
            .AsNoTracking()
            .Include(x => x.Customer)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task UpdateAsync(Offer offer)
    {
        offer.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _dbContext.Offers
            .AnyAsync(x => x.Id == id);
    }
}