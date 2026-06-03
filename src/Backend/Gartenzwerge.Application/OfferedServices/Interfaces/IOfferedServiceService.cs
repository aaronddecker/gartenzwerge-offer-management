using Gartenzwerge.Application.OfferedServices.DTOs;

namespace Gartenzwerge.Application.OfferedServices.Interfaces;

/// <summary>
/// Defines application-level use cases for managing offered services.
/// </summary>
public interface IOfferedServiceService
{
    Task<OfferedServiceDto> CreateAsync(CreateOfferedServiceRequest request);

    Task<OfferedServiceDto?> GetByIdAsync(Guid id);

    Task<IEnumerable<OfferedServiceDto>> GetAllAsync();

    Task<OfferedServiceDto?> UpdateAsync(Guid id, UpdateOfferedServiceRequest request);

    Task<bool> DeleteAsync(Guid id);
}