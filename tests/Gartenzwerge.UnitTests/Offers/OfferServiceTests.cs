using FluentAssertions;
using Gartenzwerge.Application.Customers.DTOs;
using Gartenzwerge.Application.Customers.Services;
using Gartenzwerge.Application.Offers.DTOs;
using Gartenzwerge.Application.Offers.Services;
using Gartenzwerge.Application.Common.Exceptions;
using Gartenzwerge.Domain.Enums;
using Gartenzwerge.UnitTests.Customers;

namespace Gartenzwerge.UnitTests.Offers;

/// <summary>
/// Unit tests for OfferService.
/// 
/// These tests verify offer application logic without using
/// a real database or web API.
/// </summary>
public class OfferServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldCreateOffer_WhenCustomerExists()
    {
        // Arrange
        var offerRepository = new FakeOfferRepository();
        var customerRepository = new FakeCustomerRepository();

        var customerService = new CustomerService(customerRepository);

        var customer = await customerService.CreateAsync(new CreateCustomerRequest
        {
            FirstName = "Max",
            LastName = "Mustermann",
            Email = "max.mustermann@example.com"
        });

        var offerService = new OfferService(
            offerRepository,
            customerRepository);

        var request = new CreateOfferRequest
        {
            CustomerId = customer.Id,
            ValidUntil = DateTime.UtcNow.AddDays(14),
            Notes = "Initial garden maintenance offer."
        };

        // Act
        var result = await offerService.CreateAsync(request);

        // Assert
        result.Id.Should().NotBeEmpty();
        result.CustomerId.Should().Be(customer.Id);
        result.CustomerName.Should().Be("Max Mustermann");
        result.Status.Should().Be(OfferStatus.Draft);
        result.TotalNet.Should().Be(0);
        result.OfferNumber.Should().StartWith("O-");
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowException_WhenCustomerDoesNotExist()
    {
        // Arrange
        var offerRepository = new FakeOfferRepository();
        var customerRepository = new FakeCustomerRepository();

        var offerService = new OfferService(
            offerRepository,
            customerRepository);

        var request = new CreateOfferRequest
        {
            CustomerId = Guid.NewGuid(),
            ValidUntil = DateTime.UtcNow.AddDays(14),
            Notes = "Offer for unknown customer."
        };

        // Act
        var act = async () => await offerService.CreateAsync(request);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("Customer was not found.");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnOffer_WhenOfferExists()
    {
        // Arrange
        var offerRepository = new FakeOfferRepository();
        var customerRepository = new FakeCustomerRepository();

        var customerService = new CustomerService(customerRepository);

        var customer = await customerService.CreateAsync(new CreateCustomerRequest
        {
            FirstName = "Anna",
            LastName = "Schmidt"
        });

        var offerService = new OfferService(
            offerRepository,
            customerRepository);

        var createdOffer = await offerService.CreateAsync(new CreateOfferRequest
        {
            CustomerId = customer.Id,
            ValidUntil = DateTime.UtcNow.AddDays(30)
        });

        // Act
        var result = await offerService.GetByIdAsync(createdOffer.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(createdOffer.Id);
        result.CustomerId.Should().Be(customer.Id);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateOffer_WhenOfferExists()
    {
        // Arrange
        var offerRepository = new FakeOfferRepository();
        var customerRepository = new FakeCustomerRepository();

        var customerService = new CustomerService(customerRepository);

        var customer = await customerService.CreateAsync(new CreateCustomerRequest
        {
            FirstName = "Lisa",
            LastName = "Mayer"
        });

        var offerService = new OfferService(
            offerRepository,
            customerRepository);

        var createdOffer = await offerService.CreateAsync(new CreateOfferRequest
        {
            CustomerId = customer.Id,
            ValidUntil = DateTime.UtcNow.AddDays(10),
            Notes = "Old notes"
        });

        var updateRequest = new UpdateOfferRequest
        {
            ValidUntil = DateTime.UtcNow.AddDays(20),
            Status = OfferStatus.Sent,
            Notes = "Updated notes"
        };

        // Act
        var updatedOffer = await offerService.UpdateAsync(
            createdOffer.Id,
            updateRequest);

        // Assert
        updatedOffer.Should().NotBeNull();
        updatedOffer!.Status.Should().Be(OfferStatus.Sent);
        updatedOffer.Notes.Should().Be("Updated notes");
    }

    [Fact]
    public async Task DeleteAsync_ShouldSoftDeleteOffer_WhenOfferExists()
    {
        // Arrange
        var offerRepository = new FakeOfferRepository();
        var customerRepository = new FakeCustomerRepository();

        var customerService = new CustomerService(customerRepository);

        var customer = await customerService.CreateAsync(new CreateCustomerRequest
        {
            FirstName = "Tom",
            LastName = "Becker"
        });

        var offerService = new OfferService(
            offerRepository,
            customerRepository);

        var createdOffer = await offerService.CreateAsync(new CreateOfferRequest
        {
            CustomerId = customer.Id,
            ValidUntil = DateTime.UtcNow.AddDays(10)
        });

        // Act
        var deleted = await offerService.DeleteAsync(createdOffer.Id);
        var resultAfterDelete = await offerService.GetByIdAsync(createdOffer.Id);

        // Assert
        deleted.Should().BeTrue();
        resultAfterDelete.Should().BeNull();
    }
}