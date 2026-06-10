using Gartenzwerge.Application.OfferedServices.Interfaces;
using Gartenzwerge.Application.Offers.Interfaces;
using Gartenzwerge.Application.OfferItems.DTOs;
using Gartenzwerge.Application.OfferItems.Interfaces;
using Gartenzwerge.Application.Common.Exceptions;
using Gartenzwerge.Domain.Entities;

namespace Gartenzwerge.Application.OfferItems.Services;

/// <summary>
/// Provides application-level use cases for offer item management.
/// 
/// This service contains business/application logic and depends only on
/// repository abstractions, not on Entity Framework Core directly.
/// </summary>
public class OfferItemService : IOfferItemService
{
    private readonly IOfferRepository _offerRepository;
    private readonly IOfferedServiceRepository _offeredServiceRepository;

    public OfferItemService(
        IOfferRepository offerRepository,
        IOfferedServiceRepository offeredServiceRepository)
    {
        _offerRepository = offerRepository;
        _offeredServiceRepository = offeredServiceRepository;
    }

    public async Task<OfferItemDto?> AddItemAsync(Guid offerId, CreateOfferItemRequest request)
    {
        var offer = await _offerRepository.GetByIdWithItemsAsync(offerId);

        if (offer is null)
        {
            throw new NotFoundException("Offer was not found.");
        }

        var offeredService = await _offeredServiceRepository.GetByIdAsync(request.OfferedServiceId);

        if (offeredService is null)
        {
            throw new NotFoundException("Offered service was not found.");
        }

        var unitPrice = offeredService.BasePrice;
        var totalPrice = request.Quantity * unitPrice;
        var description = offeredService.Name;

        var offerItem = new OfferItem
        {
            OfferId = offer.Id,
            OfferedServiceId = offeredService.Id,
            OfferedService = offeredService,
            Description = description,
            Quantity = request.Quantity,
            UnitPrice = unitPrice,
            TotalPrice = totalPrice
        };

        offer.Items.Add(offerItem);

        offer.TotalNet = offer.Items
            .Where(x => !x.IsDeleted)
            .Sum(x => x.TotalPrice);

        await _offerRepository.UpdateAsync(offer);

        return new OfferItemDto
        {
            Id = offerItem.Id,
            OfferId = offerItem.OfferId,
            OfferedServiceId = offerItem.OfferedServiceId,
            Description = offerItem.Description,
            Quantity = offerItem.Quantity,
            Unit = offeredService.Unit,
            UnitPrice = offerItem.UnitPrice,
            TotalPrice = offerItem.TotalPrice
        };
    }

    // Retrieves all non-deleted offer items for a given offer.
    public async Task<IReadOnlyList<OfferItemDto>> GetItemsByOfferIdAsync(Guid offerId)
    {
        var offer = await _offerRepository.GetByIdWithItemsAsync(offerId);

        if (offer is null)
        {
            throw new NotFoundException("Offer was not found.");
        }

        return offer.Items
        .Where(x => !x.IsDeleted)
        .Select(x => new OfferItemDto
        {
            Id = x.Id,
            OfferId = x.OfferId,
            OfferedServiceId = x.OfferedServiceId,
            Description = x.Description,
            Quantity = x.Quantity,
            Unit = x.OfferedService?.Unit ?? string.Empty,
            UnitPrice = x.UnitPrice,
            TotalPrice = x.TotalPrice
        })
        .ToList();
    }

    // Updates the quantity of an existing offer item and recalculates prices accordingly.
    public async Task<OfferItemDto> UpdateItemAsync(
    Guid offerId,
    Guid itemId,
    UpdateOfferItemRequest request)
    {
        var offer = await _offerRepository.GetByIdWithItemsAsync(offerId);

        if (offer is null)
        {
            throw new NotFoundException("Offer was not found.");
        }

        var item = offer.Items.FirstOrDefault(x => x.Id == itemId && !x.IsDeleted);

        if (item is null)
        {
            throw new NotFoundException("Offer item was not found.");
        }

        item.Quantity = request.Quantity;
        item.TotalPrice = item.Quantity * item.UnitPrice;

        offer.TotalNet = offer.Items
            .Where(x => !x.IsDeleted)
            .Sum(x => x.TotalPrice);

        await _offerRepository.UpdateAsync(offer);

        return new OfferItemDto
        {
            Id = item.Id,
            OfferId = item.OfferId,
            OfferedServiceId = item.OfferedServiceId,
            Description = item.Description,
            Quantity = item.Quantity,
            Unit = item.OfferedService?.Unit ?? string.Empty,
            UnitPrice = item.UnitPrice,
            TotalPrice = item.TotalPrice
        };
    }

    public async Task DeleteItemAsync(Guid offerId, Guid itemId)
    {
        var offer = await _offerRepository.GetByIdWithItemsAsync(offerId);

        if (offer is null)
        {
            throw new NotFoundException("Offer was not found.");
        }

        var item = offer.Items.FirstOrDefault(x => x.Id == itemId && !x.IsDeleted);

        if (item is null)
        {
            throw new NotFoundException("Offer item was not found.");
        }

        item.IsDeleted = true;
        item.DeletedAt = DateTime.UtcNow;

        offer.TotalNet = offer.Items
            .Where(x => !x.IsDeleted)
            .Sum(x => x.TotalPrice);

        await _offerRepository.UpdateAsync(offer);
    }
}