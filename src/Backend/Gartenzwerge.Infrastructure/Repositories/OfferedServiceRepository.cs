using Gartenzwerge.Application.OfferedServices.Interfaces;
using Gartenzwerge.Domain.Entities;
using Gartenzwerge.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gartenzwerge.Infrastructure.Repositories;

/// <summary>
/// Entity Framework Core implementation of the offered service repository.
/// 
/// This repository contains database-specific logic and keeps EF Core
/// out of the Application layer.
/// </summary>
public class OfferedServiceRepository : IOfferedServiceRepository
{
    private readonly AppDbContext _dbContext;

    public OfferedServiceRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<OfferedService> AddAsync(OfferedService offeredService)
    {
        _dbContext.OfferedServices.Add(offeredService);

        await _dbContext.SaveChangesAsync();

        return offeredService;
    }

    public async Task<OfferedService?> GetByIdAsync(Guid id)
    {
        return await _dbContext.OfferedServices
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IReadOnlyList<OfferedService>> GetAllAsync()
    {
        return await _dbContext.OfferedServices
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task UpdateAsync(OfferedService offeredService)
    {
        offeredService.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _dbContext.OfferedServices
            .AnyAsync(x => x.Id == id);
    }
}