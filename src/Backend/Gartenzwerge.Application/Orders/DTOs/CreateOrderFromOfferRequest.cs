namespace Gartenzwerge.Application.Orders.DTOs;

public class CreateOrderFromOfferRequest
{
    public DateTime? PlannedDate { get; set; }

    public string? Notes { get; set; }
}