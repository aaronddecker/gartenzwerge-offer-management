namespace Gartenzwerge.Application.OfferItems.DTOs;


/// <summary>
/// Request model for updating the quantity of an existing offer item.
/// </summary>
public class UpdateOfferItemRequest
{
    public decimal Quantity { get; set; }
}