using Gartenzwerge.Application.OfferedServices.DTOs;
using Gartenzwerge.Application.OfferedServices.Interfaces;
using Gartenzwerge.Domain.Entities;

namespace Gartenzwerge.Application.OfferedServices.Services;

/// <summary>
/// Provides application-level use cases for offered service management.
/// 
/// This service contains business/application logic and depends only on
/// repository abstractions, not on Entity Framework Core directly.
/// </summary>
public class OfferedServiceService : IOfferedServiceService
{
    private readonly IOfferedServiceRepository _offeredServiceRepository;

    public OfferedServiceService(IOfferedServiceRepository offeredServiceRepository)
    {
        _offeredServiceRepository = offeredServiceRepository;
    }

    /// <summary>
    /// Creates a new offered service.
    /// </summary>
    public async Task<OfferedServiceDto> CreateAsync(CreateOfferedServiceRequest request)
    {
        var offeredService = new OfferedService
        {
            Name = request.Name,
            Description = request.Description,
            Unit = request.Unit,
            BasePrice = request.BasePrice,
            IsActive = request.IsActive
        };

        var createdOfferedService = await _offeredServiceRepository.AddAsync(offeredService);

        return MapToDto(createdOfferedService);
    }

    /// <summary>
    /// Retrieves an offered service by its unique identifier.
    /// </summary>
    public async Task<OfferedServiceDto?> GetByIdAsync(Guid id)
    {
        var offeredService = await _offeredServiceRepository.GetByIdAsync(id);

        return offeredService is null
            ? null
            : MapToDto(offeredService);
    }

    /// <summary>
    /// Retrieves all active offered services.
    /// </summary>
    public async Task<IEnumerable<OfferedServiceDto>> GetAllAsync()
    {
        var offeredServices = await _offeredServiceRepository.GetAllAsync();

        return offeredServices.Select(MapToDto);
    }

    /// <summary>
    /// Updates an existing offered service.
    /// </summary>
    public async Task<OfferedServiceDto?> UpdateAsync(
        Guid id,
        UpdateOfferedServiceRequest request)
    {
        var offeredService = await _offeredServiceRepository.GetByIdAsync(id);

        if (offeredService is null)
        {
            return null;
        }

        offeredService.Name = request.Name;
        offeredService.Description = request.Description;
        offeredService.Unit = request.Unit;
        offeredService.BasePrice = request.BasePrice;
        offeredService.IsActive = request.IsActive;

        await _offeredServiceRepository.UpdateAsync(offeredService);

        return MapToDto(offeredService);
    }

    /// <summary>
    /// Soft-deletes an offered service.
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var offeredService = await _offeredServiceRepository.GetByIdAsync(id);

        if (offeredService is null)
        {
            return false;
        }

        offeredService.IsDeleted = true;
        offeredService.DeletedAt = DateTime.UtcNow;

        await _offeredServiceRepository.UpdateAsync(offeredService);

        return true;
    }

    /// <summary>
    /// Maps an offered service domain entity to a DTO.
    /// </summary>
    private static OfferedServiceDto MapToDto(OfferedService offeredService)
    {
        return new OfferedServiceDto
        {
            Id = offeredService.Id,
            Name = offeredService.Name,
            Description = offeredService.Description,
            Unit = offeredService.Unit,
            BasePrice = offeredService.BasePrice,
            IsActive = offeredService.IsActive
        };
    }
}