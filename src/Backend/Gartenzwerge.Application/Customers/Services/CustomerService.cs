using Gartenzwerge.Application.Customers.DTOs;
using Gartenzwerge.Application.Customers.Interfaces;
using Gartenzwerge.Domain.Entities;

namespace Gartenzwerge.Application.Customers.Services;

/// <summary>
/// Provides application-level use cases for customer management.
/// 
/// This service contains business/application logic and depends only on
/// abstractions. It does not know how customers are stored.
/// </summary>
public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    /// <summary>
    /// Creates a new customer service instance.
    /// </summary>
    /// <param name="customerRepository">
    /// Repository abstraction used to access customer data.
    /// </param>
    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    /// <summary>
    /// Creates a new customer from an incoming request.
    /// </summary>
    public async Task<CustomerDto> CreateAsync(CreateCustomerRequest request)
    {
        var customer = new Customer
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Company = request.Company,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            Street = request.Street,
            HouseNumber = request.HouseNumber,
            PostalCode = request.PostalCode,
            City = request.City,
            Notes = request.Notes
        };

        var createdCustomer = await _customerRepository.AddAsync(customer);

        return MapToDto(createdCustomer);
    }

    /// <summary>
    /// Retrieves a customer by its unique identifier.
    /// </summary>
    public async Task<CustomerDto?> GetByIdAsync(Guid id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);

        return customer is null ? null : MapToDto(customer);
    }

    /// <summary>
    /// Retrieves all active customers.
    /// </summary>
    public async Task<IEnumerable<CustomerDto>> GetAllAsync()
    {
        var customers = await _customerRepository.GetAllAsync();

        return customers.Select(MapToDto);
    }

    /// <summary>
    /// Updates an existing customer.
    /// </summary>
    public async Task<CustomerDto?> UpdateAsync(Guid id, UpdateCustomerRequest request)
    {
        var customer = await _customerRepository.GetByIdAsync(id);

        if (customer is null)
        {
            return null;
        }

        customer.FirstName = request.FirstName;
        customer.LastName = request.LastName;
        customer.Company = request.Company;
        customer.PhoneNumber = request.PhoneNumber;
        customer.Email = request.Email;
        customer.Street = request.Street;
        customer.HouseNumber = request.HouseNumber;
        customer.PostalCode = request.PostalCode;
        customer.City = request.City;
        customer.Notes = request.Notes;

        await _customerRepository.UpdateAsync(customer);

        return MapToDto(customer);
    }

    /// <summary>
    /// Soft-deletes an existing customer.
    /// 
    /// The record remains in the database but is excluded from normal queries
    /// through the global query filter configured in the Infrastructure layer.
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);

        if (customer is null)
        {
            return false;
        }

        customer.IsDeleted = true;
        customer.DeletedAt = DateTime.UtcNow;

        await _customerRepository.UpdateAsync(customer);

        return true;
    }

    /// <summary>
    /// Maps a customer domain entity to a DTO.
    /// 
    /// DTOs keep API contracts independent from the internal domain model.
    /// </summary>
    private static CustomerDto MapToDto(Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Company = customer.Company,
            PhoneNumber = customer.PhoneNumber,
            Email = customer.Email,
            Street = customer.Street,
            HouseNumber = customer.HouseNumber,
            PostalCode = customer.PostalCode,
            City = customer.City,
            Notes = customer.Notes
        };
    }
}