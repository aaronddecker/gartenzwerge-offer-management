namespace Gartenzwerge.Application.OfferedServices.DTOs;

/// <summary>
/// Request model for creating a new offered service.
///
/// Only fields that are allowed to be set by API clients are included.
/// </summary>
public class CreateOfferedServiceRequest
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Unit { get; set; } = string.Empty;

    public decimal BasePrice { get; set; }

    public bool IsActive { get; set; } = true;
}