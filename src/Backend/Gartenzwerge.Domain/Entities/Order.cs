using Gartenzwerge.Domain.Common;
using Gartenzwerge.Domain.Enums;

namespace Gartenzwerge.Domain.Entities;

public class Order : BaseEntity
{
    public Guid OfferId { get; set; }
    public Offer Offer { get; set; } = null!;

    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public OrderStatus Status { get; set; } = OrderStatus.Planned;

    public DateTime? PlannedDate { get; set; }
    public DateTime? CompletedAt { get; set; }

    public string? Notes { get; set; }
}