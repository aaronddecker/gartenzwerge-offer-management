using Gartenzwerge.Domain.Common;

namespace Gartenzwerge.Domain.Entities;

public class OfferItem : BaseEntity
{
    public Guid OfferId { get; set; }
    public Offer Offer { get; set; } = null!;

    public Guid ServiceId { get; set; }
    public Service Service { get; set; } = null!;

    public string Description { get; set; } = string.Empty;

    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}