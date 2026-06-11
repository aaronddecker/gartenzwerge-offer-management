using Gartenzwerge.Application.Orders.DTOs;
using Gartenzwerge.Application.Orders.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gartenzwerge.API.Controllers;

/// <summary>
/// This controller provides endpoints for managing orders, specifically for creating orders from accepted offers.
/// </summary>
[ApiController]
[Route("api/offers/{offerId:guid}/order")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateFromOffer(
        Guid offerId,
        CreateOrderFromOfferRequest request)
    {
        var order = await _orderService.CreateFromOfferAsync(offerId, request);

        return Created($"/api/orders/{order.Id}", order);
    }
}