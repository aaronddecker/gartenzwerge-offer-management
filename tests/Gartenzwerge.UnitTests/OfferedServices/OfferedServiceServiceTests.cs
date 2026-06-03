using FluentAssertions;
using Gartenzwerge.Application.OfferedServices.DTOs;
using Gartenzwerge.Application.OfferedServices.Services;

namespace Gartenzwerge.UnitTests.OfferedServices;

/// <summary>
/// Unit tests for OfferedServiceService.
/// 
/// These tests verify offered service application logic without using
/// a real database or web API.
/// </summary>
public class OfferedServiceServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldCreateOfferedService_WhenRequestIsValid()
    {
        // Arrange
        var repository = new FakeOfferedServiceRepository();
        var service = new OfferedServiceService(repository);

        var request = new CreateOfferedServiceRequest
        {
            Name = "Lawn mowing",
            Description = "Mowing lawn areas based on square meters.",
            Unit = "m²",
            BasePrice = 0.35m,
            IsActive = true
        };

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        result.Id.Should().NotBeEmpty();
        result.Name.Should().Be("Lawn mowing");
        result.Unit.Should().Be("m²");
        result.BasePrice.Should().Be(0.35m);
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnOfferedService_WhenOfferedServiceExists()
    {
        // Arrange
        var repository = new FakeOfferedServiceRepository();
        var service = new OfferedServiceService(repository);

        var createdService = await service.CreateAsync(new CreateOfferedServiceRequest
        {
            Name = "Hedge cutting",
            Unit = "m",
            BasePrice = 7.00m
        });

        // Act
        var result = await service.GetByIdAsync(createdService.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(createdService.Id);
        result.Name.Should().Be("Hedge cutting");
        result.Unit.Should().Be("m");
        result.BasePrice.Should().Be(7.00m);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenOfferedServiceDoesNotExist()
    {
        // Arrange
        var repository = new FakeOfferedServiceRepository();
        var service = new OfferedServiceService(repository);

        // Act
        var result = await service.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateOfferedService_WhenOfferedServiceExists()
    {
        // Arrange
        var repository = new FakeOfferedServiceRepository();
        var service = new OfferedServiceService(repository);

        var createdService = await service.CreateAsync(new CreateOfferedServiceRequest
        {
            Name = "Old Service",
            Unit = "m²",
            BasePrice = 1.00m
        });

        var updateRequest = new UpdateOfferedServiceRequest
        {
            Name = "Updated Service",
            Description = "Updated description",
            Unit = "m²",
            BasePrice = 2.50m,
            IsActive = true
        };

        // Act
        var result = await service.UpdateAsync(createdService.Id, updateRequest);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Updated Service");
        result.Description.Should().Be("Updated description");
        result.BasePrice.Should().Be(2.50m);
    }

    [Fact]
    public async Task DeleteAsync_ShouldSoftDeleteOfferedService_WhenOfferedServiceExists()
    {
        // Arrange
        var repository = new FakeOfferedServiceRepository();
        var service = new OfferedServiceService(repository);

        var createdService = await service.CreateAsync(new CreateOfferedServiceRequest
        {
            Name = "Green waste disposal",
            Unit = "flat",
            BasePrice = 20.00m
        });

        // Act
        var deleted = await service.DeleteAsync(createdService.Id);
        var resultAfterDelete = await service.GetByIdAsync(createdService.Id);

        // Assert
        deleted.Should().BeTrue();
        resultAfterDelete.Should().BeNull();
    }
}