using FluentAssertions;
using Gartenzwerge.Application.Customers.DTOs;
using Gartenzwerge.Application.Customers.Services;

namespace Gartenzwerge.UnitTests.Customers;

/// <summary>
/// Unit tests for CustomerService.
/// 
/// These tests verify application-level behavior without using
/// a real database or web API.
/// </summary>
public class CustomerServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldCreateCustomer_WhenRequestIsValid()
    {
        // Arrange
        var repository = new FakeCustomerRepository();
        var service = new CustomerService(repository);

        var request = new CreateCustomerRequest
        {
            FirstName = "Max",
            LastName = "Mustermann",
            Email = "max.mustermann@example.com",
            City = "Eberdingen"
        };

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        result.Id.Should().NotBeEmpty();
        result.FirstName.Should().Be("Max");
        result.LastName.Should().Be("Mustermann");
        result.Email.Should().Be("max.mustermann@example.com");
        result.City.Should().Be("Eberdingen");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCustomer_WhenCustomerExists()
    {
        // Arrange
        var repository = new FakeCustomerRepository();
        var service = new CustomerService(repository);

        var createdCustomer = await service.CreateAsync(new CreateCustomerRequest
        {
            FirstName = "Anna",
            LastName = "Schmidt"
        });

        // Act
        var result = await service.GetByIdAsync(createdCustomer.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(createdCustomer.Id);
        result.FirstName.Should().Be("Anna");
        result.LastName.Should().Be("Schmidt");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenCustomerDoesNotExist()
    {
        // Arrange
        var repository = new FakeCustomerRepository();
        var service = new CustomerService(repository);

        // Act
        var result = await service.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldSoftDeleteCustomer_WhenCustomerExists()
    {
        // Arrange
        var repository = new FakeCustomerRepository();
        var service = new CustomerService(repository);

        var createdCustomer = await service.CreateAsync(new CreateCustomerRequest
        {
            FirstName = "Lisa",
            LastName = "Mayer"
        });

        // Act
        var deleted = await service.DeleteAsync(createdCustomer.Id);
        var resultAfterDelete = await service.GetByIdAsync(createdCustomer.Id);

        // Assert
        deleted.Should().BeTrue();
        resultAfterDelete.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateCustomer_WhenCustomerExists()
    {
        // Arrange
        var repository = new FakeCustomerRepository();
        var service = new CustomerService(repository);

        var createdCustomer = await service.CreateAsync(new CreateCustomerRequest
        {
            FirstName = "Old",
            LastName = "Name",
            City = "Old City"
        });

        var updateRequest = new UpdateCustomerRequest
        {
            FirstName = "New",
            LastName = "Name",
            City = "New City"
        };

        // Act
        var updatedCustomer = await service.UpdateAsync(
            createdCustomer.Id,
            updateRequest);

        // Assert
        updatedCustomer.Should().NotBeNull();
        updatedCustomer!.FirstName.Should().Be("New");
        updatedCustomer.LastName.Should().Be("Name");
        updatedCustomer.City.Should().Be("New City");
    }
}