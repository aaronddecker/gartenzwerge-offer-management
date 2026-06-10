using System;

namespace Gartenzwerge.Application.OfferItems.DTOs;

/// <summary>
/// Request model for creating a new item in an offer.
/// 
/// Only fields that are allowed to be set by API clients are included.
/// </summary>
public class CreateOfferItemRequest
{
    public Guid OfferedServiceId { get; set; }

    public decimal Quantity { get; set; }
}