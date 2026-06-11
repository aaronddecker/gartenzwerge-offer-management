using Gartenzwerge.Domain.Enums;

namespace Gartenzwerge.Application.Orders.DTOs;

public class OrderDto
{
    public Guid Id { get; set; }

    public Guid OfferId { get; set; }

    public Guid CustomerId { get; set; }

    public OrderStatus Status { get; set; }

    public DateTime? PlannedDate { get; set; }

    public DateTime? CompletedAt { get; set; }

    public string? Notes { get; set; }
}