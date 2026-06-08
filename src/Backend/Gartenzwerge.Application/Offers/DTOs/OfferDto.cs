using Gartenzwerge.Domain.Enums;

namespace Gartenzwerge.Application.Offers.DTOs;

/// <summary>
/// Data transfer object returned by the API for offers.
/// </summary>
public class OfferDto
{
    public Guid Id { get; set; }

    public string OfferNumber { get; set; } = string.Empty;

    public Guid CustomerId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime ValidUntil { get; set; }

    public OfferStatus Status { get; set; }

    public decimal TotalNet { get; set; }

    public string? Notes { get; set; }
}