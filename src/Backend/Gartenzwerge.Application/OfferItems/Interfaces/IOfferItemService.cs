using Gartenzwerge.Application.OfferItems.DTOs;

namespace Gartenzwerge.Application.OfferItems.Interfaces;


/// <summary>
/// Defines application-level offer item use cases.
/// 
/// Controllers depend on this abstraction instead of directly calling
/// concrete services or repositories.
/// </summary>
public interface IOfferItemService
{
    Task<OfferItemDto> AddItemAsync(Guid offerId, CreateOfferItemRequest request);

    Task<IReadOnlyList<OfferItemDto>> GetItemsByOfferIdAsync(Guid offerId);

    Task<OfferItemDto> UpdateItemAsync(
    Guid offerId,
    Guid itemId,
    UpdateOfferItemRequest request);

    Task DeleteItemAsync(Guid offerId, Guid itemId);
}