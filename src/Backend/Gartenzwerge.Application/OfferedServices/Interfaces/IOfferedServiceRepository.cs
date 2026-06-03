using Gartenzwerge.Domain.Entities;

namespace Gartenzwerge.Application.OfferedServices.Interfaces;

/// <summary>
/// Defines persistence operations for offered service entities.
///
/// The Application layer depends on this abstraction and does not know
/// whether data is stored with EF Core, PostgreSQL or another technology.
/// </summary>
public interface IOfferedServiceRepository
{
    Task<OfferedService> AddAsync(OfferedService offeredService);

    Task<OfferedService?> GetByIdAsync(Guid id);

    Task<IReadOnlyList<OfferedService>> GetAllAsync();

    Task UpdateAsync(OfferedService offeredService);

    Task<bool> ExistsAsync(Guid id);
}