using Gartenzwerge.Application.OfferedServices.DTOs;
using Gartenzwerge.Application.OfferedServices.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gartenzwerge.API.Controllers;

/// <summary>
/// Provides REST endpoints for managing offered services.
///
/// Offered services represent business services such as lawn mowing,
/// hedge cutting or green waste disposal.
/// </summary>
[ApiController]
[Route("api/offered-services")]
public class OfferedServicesController : ControllerBase
{
    private readonly IOfferedServiceService _offeredServiceService;

    public OfferedServicesController(IOfferedServiceService offeredServiceService)
    {
        _offeredServiceService = offeredServiceService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OfferedServiceDto>>> GetAll()
    {
        var offeredServices = await _offeredServiceService.GetAllAsync();

        return Ok(offeredServices);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OfferedServiceDto>> GetById(Guid id)
    {
        var offeredService = await _offeredServiceService.GetByIdAsync(id);

        if (offeredService is null)
        {
            return NotFound();
        }

        return Ok(offeredService);
    }

    [HttpPost]
    public async Task<ActionResult<OfferedServiceDto>> Create(
        CreateOfferedServiceRequest request)
    {
        var offeredService = await _offeredServiceService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = offeredService.Id },
            offeredService);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<OfferedServiceDto>> Update(
        Guid id,
        UpdateOfferedServiceRequest request)
    {
        var offeredService = await _offeredServiceService.UpdateAsync(id, request);

        if (offeredService is null)
        {
            return NotFound();
        }

        return Ok(offeredService);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _offeredServiceService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}