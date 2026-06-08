using Gartenzwerge.Application.Offers.DTOs;

namespace Gartenzwerge.Application.Offers.Interfaces;

/// <summary>
/// Defines application-level use cases for managing offers.
/// </summary>
public interface IOfferService
{
    Task<OfferDto> CreateAsync(CreateOfferRequest request);

    Task<OfferDto?> GetByIdAsync(Guid id);

    Task<IEnumerable<OfferDto>> GetAllAsync();

    Task<OfferDto?> UpdateAsync(Guid id, UpdateOfferRequest request);

    Task<bool> DeleteAsync(Guid id);
}