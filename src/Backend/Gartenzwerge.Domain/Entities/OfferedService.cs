using Gartenzwerge.Domain.Common;

namespace Gartenzwerge.Domain.Entities;

/// <summary>
/// Represents a service offered by the landscaping business,
/// such as lawn mowing, hedge cutting or green waste disposal.
/// </summary>
public class OfferedService : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Unit { get; set; } = string.Empty;

    public decimal BasePrice { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<OfferItem> OfferItems { get; set; } = new List<OfferItem>();
}