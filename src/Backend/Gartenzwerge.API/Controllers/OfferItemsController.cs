using Gartenzwerge.Application.OfferItems.Interfaces;
using Gartenzwerge.Application.OfferItems.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Gartenzwerge.API.Controllers;

/// <summary>
/// Provides REST endpoints for managing offer items.
/// 
/// These endpoints are nested under offers, as offer items are always associated with a specific offer.
/// </summary>
[ApiController]
[Route("api/offers/{offerId:guid}/items")]
public class OfferItemsController : ControllerBase
{
    private readonly IOfferItemService _offerItemService;

    public OfferItemsController(IOfferItemService offerItemService)
    {
        _offerItemService = offerItemService;
    }

    [HttpPost]
    public async Task<ActionResult<OfferItemDto>> AddItem(
    Guid offerId,
    CreateOfferItemRequest request)
    {
        var offerItem = await _offerItemService.AddItemAsync(offerId, request);

        if (offerItem is null)
        {
            return NotFound();
        }

        return Created(
            $"/api/offers/{offerId}/items/{offerItem.Id}",
            offerItem);
    }
}