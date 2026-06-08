using Gartenzwerge.Domain.Enums;

namespace Gartenzwerge.Application.Offers.DTOs;

/// <summary>
/// Request model for updating an existing offer.
/// </summary>
public class UpdateOfferRequest
{
    public DateTime ValidUntil { get; set; }

    public OfferStatus Status { get; set; }

    public string? Notes { get; set; }
}