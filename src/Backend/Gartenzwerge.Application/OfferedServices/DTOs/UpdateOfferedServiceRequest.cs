namespace Gartenzwerge.Application.OfferedServices.DTOs;

/// <summary>
/// Request model for updating an existing offered service.
/// </summary>
public class UpdateOfferedServiceRequest
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Unit { get; set; } = string.Empty;

    public decimal BasePrice { get; set; }

    public bool IsActive { get; set; }
}