using Gartenzwerge.Domain.Common;

namespace Gartenzwerge.Domain.Entities;

/// <summary>
/// Represents a single line item within an offer.
///
/// Each offer item references one offered service and stores
/// the quantity, unit price and calculated total price.
/// </summary>
public class OfferItem : BaseEntity
{
    public Guid OfferId { get; set; }

    public Offer Offer { get; set; } = null!;

    public Guid OfferedServiceId { get; set; }

    public OfferedService OfferedService { get; set; } = null!;

    public string Description { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }
}