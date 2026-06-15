using Gartenzwerge.Application.Offers.DTOs;
using Gartenzwerge.Application.Offers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Gartenzwerge.API.Controllers;

/// <summary>
/// Provides REST endpoints for managing offers.
/// 
/// Offers represent commercial proposals sent to customers.
/// </summary>
[ApiController]
[Route("api/offers")]
[Authorize]
public class OffersController : ControllerBase
{
    private readonly IOfferService _offerService;

    public OffersController(IOfferService offerService)
    {
        _offerService = offerService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OfferDto>>> GetAll()
    {
        var offers = await _offerService.GetAllAsync();

        return Ok(offers);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OfferDto>> GetById(Guid id)
    {
        var offer = await _offerService.GetByIdAsync(id);

        if (offer is null)
        {
            return NotFound();
        }

        return Ok(offer);
    }

    [HttpPost]
    public async Task<ActionResult<OfferDto>> Create(CreateOfferRequest request)
    {
        var offer = await _offerService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = offer.Id },
            offer);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<OfferDto>> Update(
        Guid id,
        UpdateOfferRequest request)
    {
        var offer = await _offerService.UpdateAsync(id, request);

        if (offer is null)
        {
            return NotFound();
        }

        return Ok(offer);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _offerService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}