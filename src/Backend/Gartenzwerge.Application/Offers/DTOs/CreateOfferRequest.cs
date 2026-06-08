namespace Gartenzwerge.Application.Offers.DTOs;

/// <summary>
/// Request model for creating a new offer.
/// 
/// Offer numbers and status transitions are handled by the application layer.
/// </summary>
public class CreateOfferRequest
{
    public Guid CustomerId { get; set; }

    public DateTime ValidUntil { get; set; }

    public string? Notes { get; set; }
}