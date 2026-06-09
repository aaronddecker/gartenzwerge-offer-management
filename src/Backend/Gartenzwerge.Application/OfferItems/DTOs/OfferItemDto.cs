namespace Gartenzwerge.Application.OfferItems.DTOs;

/// <summary>
/// Data transfer object returned by the API for offer items.
/// </summary>
public class OfferItemDto
{
    public Guid Id { get; set; }

    public Guid OfferId { get; set; }

    public Guid OfferedServiceId { get; set; }

    public string Description { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public string Unit { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }
}