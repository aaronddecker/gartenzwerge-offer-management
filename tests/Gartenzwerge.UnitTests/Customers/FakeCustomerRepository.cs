using Gartenzwerge.Application.Customers.Interfaces;
using Gartenzwerge.Domain.Entities;

namespace Gartenzwerge.UnitTests.Customers;

/// <summary>
/// In-memory test implementation of ICustomerRepository.
/// 
/// This fake repository allows testing CustomerService without requiring
/// Entity Framework Core or a real PostgreSQL database.
/// </summary>
public class FakeCustomerRepository : ICustomerRepository
{
    private readonly List<Customer> _customers = [];

    public Task<Customer> AddAsync(Customer customer)
    {
        _customers.Add(customer);

        return Task.FromResult(customer);
    }

    public Task<Customer?> GetByIdAsync(Guid id)
    {
        var customer = _customers.FirstOrDefault(x => x.Id == id && !x.IsDeleted);

        return Task.FromResult(customer);
    }

    public Task<IReadOnlyList<Customer>> GetAllAsync()
    {
        IReadOnlyList<Customer> customers = _customers
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .ToList();

        return Task.FromResult(customers);
    }

    public Task UpdateAsync(Customer customer)
    {
        customer.UpdatedAt = DateTime.UtcNow;

        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(Guid id)
    {
        var exists = _customers.Any(x => x.Id == id && !x.IsDeleted);

        return Task.FromResult(exists);
    }
}