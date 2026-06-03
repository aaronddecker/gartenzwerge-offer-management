using Gartenzwerge.Domain.Common;
using Gartenzwerge.Domain.Enums;

namespace Gartenzwerge.Domain.Entities;

public class Offer : BaseEntity
{
    public string OfferNumber { get; set; } = string.Empty;

    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public DateTime ValidUntil { get; set; }

    public OfferStatus Status { get; set; } = OfferStatus.Draft;

    public decimal TotalNet { get; set; }

    public string? Notes { get; set; }

    public ICollection<OfferItem> Items { get; set; } = new List<OfferItem>();

    public Order? Order { get; set; }
}