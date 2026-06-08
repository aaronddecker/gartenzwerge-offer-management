using Gartenzwerge.Application.Customers.Interfaces;
using Gartenzwerge.Application.Offers.DTOs;
using Gartenzwerge.Application.Offers.Interfaces;
using Gartenzwerge.Domain.Entities;
using Gartenzwerge.Domain.Enums;

namespace Gartenzwerge.Application.Offers.Services;

/// <summary>
/// Provides application-level use cases for offer management.
/// 
/// This service coordinates offer creation, updates and soft deletion.
/// It depends only on repository abstractions.
/// </summary>
public class OfferService : IOfferService
{
    private readonly IOfferRepository _offerRepository;
    private readonly ICustomerRepository _customerRepository;

    public OfferService(
        IOfferRepository offerRepository,
        ICustomerRepository customerRepository)
    {
        _offerRepository = offerRepository;
        _customerRepository = customerRepository;
    }

    public async Task<OfferDto> CreateAsync(CreateOfferRequest request)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId);

        if (customer is null)
        {
            throw new InvalidOperationException("Customer does not exist.");
        }

        var offer = new Offer
        {
            OfferNumber = GenerateOfferNumber(),
            CustomerId = request.CustomerId,
            ValidUntil = request.ValidUntil,
            Status = OfferStatus.Draft,
            TotalNet = 0,
            Notes = request.Notes
        };

        var createdOffer = await _offerRepository.AddAsync(offer);

        createdOffer.Customer = customer;

        return MapToDto(createdOffer);
    }

    public async Task<OfferDto?> GetByIdAsync(Guid id)
    {
        var offer = await _offerRepository.GetByIdAsync(id);

        return offer is null ? null : MapToDto(offer);
    }

    public async Task<IEnumerable<OfferDto>> GetAllAsync()
    {
        var offers = await _offerRepository.GetAllAsync();

        return offers.Select(MapToDto);
    }

    public async Task<OfferDto?> UpdateAsync(Guid id, UpdateOfferRequest request)
    {
        var offer = await _offerRepository.GetByIdAsync(id);

        if (offer is null)
        {
            return null;
        }

        offer.ValidUntil = request.ValidUntil;
        offer.Status = request.Status;
        offer.Notes = request.Notes;

        await _offerRepository.UpdateAsync(offer);

        return MapToDto(offer);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var offer = await _offerRepository.GetByIdAsync(id);

        if (offer is null)
        {
            return false;
        }

        offer.IsDeleted = true;
        offer.DeletedAt = DateTime.UtcNow;

        await _offerRepository.UpdateAsync(offer);

        return true;
    }

    /// <summary>
    /// Generates a simple offer number.
    /// 
    /// This is intentionally simple for the current version.
    /// A later version can replace it with a database-backed sequence.
    /// </summary>
    private static string GenerateOfferNumber()
    {
        return $"O-{DateTime.UtcNow:yyyyMMdd-HHmmss}";
    }

    private static OfferDto MapToDto(Offer offer)
    {
        return new OfferDto
        {
            Id = offer.Id,
            OfferNumber = offer.OfferNumber,
            CustomerId = offer.CustomerId,
            CustomerName = offer.Customer is null
                ? string.Empty
                : $"{offer.Customer.FirstName} {offer.Customer.LastName}".Trim(),
            CreatedAt = offer.CreatedAt,
            ValidUntil = offer.ValidUntil,
            Status = offer.Status,
            TotalNet = offer.TotalNet,
            Notes = offer.Notes
        };
    }
}