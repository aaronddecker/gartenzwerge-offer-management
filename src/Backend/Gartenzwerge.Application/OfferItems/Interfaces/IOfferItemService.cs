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
    Task<OfferItemDto?> AddItemAsync(Guid offerId, CreateOfferItemRequest request);
}