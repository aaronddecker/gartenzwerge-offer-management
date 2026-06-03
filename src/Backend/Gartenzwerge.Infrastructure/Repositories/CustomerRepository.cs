using Gartenzwerge.Application.Customers.Interfaces;
using Gartenzwerge.Domain.Entities;
using Gartenzwerge.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gartenzwerge.Infrastructure.Repositories;

/// <summary>
/// Entity Framework Core implementation of the customer repository.
///
/// This repository is responsible for all database interactions
/// related to customer entities.
///
/// The Application layer depends only on the repository interface,
/// while the Infrastructure layer contains the actual EF Core implementation.
/// </summary>
public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _dbContext;

    /// <summary>
    /// Creates a new repository instance.
    /// </summary>
    /// <param name="dbContext">
    /// Entity Framework Core database context.
    /// </param>
    public CustomerRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Persists a new customer in the database.
    /// </summary>
    /// <param name="customer">Customer entity to create.</param>
    /// <returns>The created customer entity.</returns>
    public async Task<Customer> AddAsync(Customer customer)
    {
        _dbContext.Customers.Add(customer);

        await _dbContext.SaveChangesAsync();

        return customer;
    }

    /// <summary>
    /// Retrieves a customer by its unique identifier.
    /// </summary>
    /// <param name="id">Customer identifier.</param>
    /// <returns>The customer if found; otherwise null.</returns>
    public async Task<Customer?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Customers
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <summary>
    /// Retrieves all active customers ordered alphabetically.
    /// </summary>
    /// <returns>A read-only collection of customers.</returns>
    public async Task<IReadOnlyList<Customer>> GetAllAsync()
    {
        return await _dbContext.Customers
            .AsNoTracking()
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .ToListAsync();
    }

    /// <summary>
    /// Persists modifications of an existing customer.
    /// </summary>
    /// <param name="customer">Updated customer entity.</param>
    public async Task UpdateAsync(Customer customer)
    {
        customer.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Checks whether a customer exists.
    /// </summary>
    /// <param name="id">Customer identifier.</param>
    /// <returns>
    /// True if the customer exists; otherwise false.
    /// </returns>
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _dbContext.Customers
            .AnyAsync(x => x.Id == id);
    }
}