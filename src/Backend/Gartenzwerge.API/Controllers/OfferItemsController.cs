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

    // POST /api/offers/{offerId}/items
    [HttpPost]
    public async Task<ActionResult<OfferItemDto>> AddItem(
    Guid offerId,
    CreateOfferItemRequest request)
    {
        var offerItem = await _offerItemService.AddItemAsync(offerId, request);

        return Created(
            $"/api/offers/{offerId}/items/{offerItem.Id}",
            offerItem);
    }

    // GET /api/offers/{offerId}/items
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OfferItemDto>>> GetItems(Guid offerId)
    {
        var items = await _offerItemService.GetItemsByOfferIdAsync(offerId);

        return Ok(items);
    }

    // PUT /api/offers/{offerId}/items/{itemId}
    [HttpPut("{itemId:guid}")]
    public async Task<ActionResult<OfferItemDto>> UpdateItem(
    Guid offerId,
    Guid itemId,
    UpdateOfferItemRequest request)
    {
        var updatedItem = await _offerItemService.UpdateItemAsync(
            offerId,
            itemId,
            request);

        return Ok(updatedItem);
    }

    // DELETE /api/offers/{offerId}/items/{itemId}
    [HttpDelete("{itemId:guid}")]
    public async Task<IActionResult> DeleteItem(
    Guid offerId,
    Guid itemId)
    {
        await _offerItemService.DeleteItemAsync(offerId, itemId);

        return NoContent();
    }
}