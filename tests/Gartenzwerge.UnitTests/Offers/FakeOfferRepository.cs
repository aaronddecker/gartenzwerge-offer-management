using Gartenzwerge.Application.Offers.Interfaces;
using Gartenzwerge.Domain.Entities;

namespace Gartenzwerge.UnitTests.Offers;

/// <summary>
/// In-memory test implementation of IOfferRepository.
/// 
/// This fake repository allows testing OfferService without
/// Entity Framework Core or a real PostgreSQL database.
/// </summary>
public class FakeOfferRepository : IOfferRepository
{
    private readonly List<Offer> _offers = [];

    public Task<Offer> AddAsync(Offer offer)
    {
        _offers.Add(offer);

        return Task.FromResult(offer);
    }

    public Task<Offer?> GetByIdAsync(Guid id)
    {
        var offer = _offers.FirstOrDefault(x => x.Id == id && !x.IsDeleted);

        return Task.FromResult(offer);
    }

    public Task<Offer?> GetByIdWithItemsAsync(Guid id)
    {
        var offer = _offers.FirstOrDefault(x => x.Id == id && !x.IsDeleted);

        return Task.FromResult(offer);
    }

    public Task<IReadOnlyList<Offer>> GetAllAsync()
    {
        IReadOnlyList<Offer> offers = _offers
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .ToList();

        return Task.FromResult(offers);
    }

    public Task UpdateAsync(Offer offer)
    {
        offer.UpdatedAt = DateTime.UtcNow;

        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(Guid id)
    {
        var exists = _offers.Any(x => x.Id == id && !x.IsDeleted);

        return Task.FromResult(exists);
    }
}