using Gartenzwerge.Domain.Enums;

namespace Gartenzwerge.Application.Orders.DTOs;

/// <summary>
/// This class represents the data transfer object for updating an existing order, allowing changes to the order's status, planned date, and notes.
/// </summary>
public class UpdateOrderRequest
{
    public OrderStatus Status { get; set; }

    public DateTime? PlannedDate { get; set; }

    public string? Notes { get; set; }
}