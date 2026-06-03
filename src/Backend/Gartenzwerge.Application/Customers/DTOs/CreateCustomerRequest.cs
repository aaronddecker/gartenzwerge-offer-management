namespace Gartenzwerge.Application.Customers.DTOs;

public class CreateCustomerRequest
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string? Company { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? Street { get; set; }

    public string? HouseNumber { get; set; }

    public string? PostalCode { get; set; }

    public string? City { get; set; }

    public string? Notes { get; set; }
}