using Gartenzwerge.Application.OfferedServices.Interfaces;
using Gartenzwerge.Domain.Entities;

namespace Gartenzwerge.UnitTests.OfferedServices;

/// <summary>
/// In-memory test implementation of IOfferedServiceRepository.
/// 
/// This fake repository allows testing OfferedServiceService without
/// Entity Framework Core or a real PostgreSQL database.
/// </summary>
public class FakeOfferedServiceRepository : IOfferedServiceRepository
{
	private readonly List<OfferedService> _offeredServices = [];

	public Task<OfferedService> AddAsync(OfferedService offeredService)
	{
		_offeredServices.Add(offeredService);

		return Task.FromResult(offeredService);
	}

	public Task<OfferedService?> GetByIdAsync(Guid id)
	{
		var offeredService = _offeredServices
			.FirstOrDefault(x => x.Id == id && !x.IsDeleted);

		return Task.FromResult(offeredService);
	}

	public Task<IReadOnlyList<OfferedService>> GetAllAsync()
	{
		IReadOnlyList<OfferedService> offeredServices = _offeredServices
			.Where(x => !x.IsDeleted)
			.OrderBy(x => x.Name)
			.ToList();

		return Task.FromResult(offeredServices);
	}

	public Task UpdateAsync(OfferedService offeredService)
	{
		offeredService.UpdatedAt = DateTime.UtcNow;

		return Task.CompletedTask;
	}

	public Task<bool> ExistsAsync(Guid id)
	{
		var exists = _offeredServices.Any(x => x.Id == id && !x.IsDeleted);

		return Task.FromResult(exists);
	}
}