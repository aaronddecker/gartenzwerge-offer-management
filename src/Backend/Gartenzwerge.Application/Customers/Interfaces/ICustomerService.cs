using Gartenzwerge.Application.Customers.DTOs;

namespace Gartenzwerge.Application.Customers.Interfaces;

/// <summary>
/// Defines application-level customer use cases.
/// 
/// Controllers depend on this abstraction instead of directly calling
/// concrete services or repositories.
/// </summary>
public interface ICustomerService
{
    Task<CustomerDto> CreateAsync(CreateCustomerRequest request);

    Task<CustomerDto?> GetByIdAsync(Guid id);

    Task<IEnumerable<CustomerDto>> GetAllAsync();

    Task<CustomerDto?> UpdateAsync(Guid id, UpdateCustomerRequest request);

    Task<bool> DeleteAsync(Guid id);
}