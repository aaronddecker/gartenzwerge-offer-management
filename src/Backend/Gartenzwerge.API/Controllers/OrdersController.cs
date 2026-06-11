using Gartenzwerge.Application.Orders.DTOs;
using Gartenzwerge.Application.Orders.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gartenzwerge.API.Controllers;

/// <summary>
/// This controller provides endpoints for managing orders, specifically for creating orders from accepted offers.
/// </summary>
[ApiController]
[Route("api")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    // POST /api/offers/{offerId}/order
    [HttpPost("offers/{offerId:guid}/order")]
    public async Task<ActionResult<OrderDto>> CreateFromOffer(
    Guid offerId,
    CreateOrderFromOfferRequest request)
    {
        var order = await _orderService.CreateFromOfferAsync(offerId, request);

        return Created($"/api/orders/{order.Id}", order);
    }

    // GET /api/orders
    [HttpGet("orders")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll()
    {
        var orders = await _orderService.GetAllAsync();

        return Ok(orders);
    }

    // GET /api/orders/{id}
    [HttpGet("orders/{id:guid}")]
    public async Task<ActionResult<OrderDto>> GetById(Guid id)
    {
        var order = await _orderService.GetByIdAsync(id);

        return Ok(order);
    }

    // PUT /api/orders/{id}
    [HttpPut("orders/{id:guid}")]
    public async Task<ActionResult<OrderDto>> Update(
        Guid id,
        UpdateOrderRequest request)
    {
        var updatedOrder = await _orderService.UpdateAsync(id, request);

        return Ok(updatedOrder);
    }

    // DELETE /api/orders/{id}
    [HttpDelete("orders/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _orderService.DeleteAsync(id);

        return NoContent();
    }
}