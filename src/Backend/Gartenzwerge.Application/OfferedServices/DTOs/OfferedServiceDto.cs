namespace Gartenzwerge.Application.OfferedServices.DTOs;

/// <summary>
/// Data transfer object returned by the API for offered services.
/// </summary>
public class OfferedServiceDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Unit { get; set; } = string.Empty;

    public decimal BasePrice { get; set; }

    public bool IsActive { get; set; }
}