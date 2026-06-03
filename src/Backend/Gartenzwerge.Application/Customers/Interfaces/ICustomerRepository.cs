using Gartenzwerge.Domain.Entities;

namespace Gartenzwerge.Application.Customers.Interfaces;

/// <summary>
/// Defines persistence operations for customer entities.
///
/// The Application layer depends on this abstraction so it does not need
/// to know whether the data is stored with EF Core, PostgreSQL or another technology.
/// </summary>
public interface ICustomerRepository
{
    Task<Customer> AddAsync(Customer customer);

    Task<Customer?> GetByIdAsync(Guid id);

    Task<IReadOnlyList<Customer>> GetAllAsync();

    Task UpdateAsync(Customer customer);

    Task<bool> ExistsAsync(Guid id);
}