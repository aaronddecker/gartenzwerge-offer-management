using Gartenzwerge.Domain.Entities;

namespace Gartenzwerge.Application.Offers.Interfaces;

/// <summary>
/// Defines persistence operations for offer entities.
/// 
/// The Application layer depends on this abstraction instead of
/// directly depending on Entity Framework Core.
/// </summary>
public interface IOfferRepository
{
    Task<Offer> AddAsync(Offer offer);

    Task<Offer?> GetByIdAsync(Guid id);

    Task<IReadOnlyList<Offer>> GetAllAsync();

    Task UpdateAsync(Offer offer);

    Task<bool> ExistsAsync(Guid id);
}