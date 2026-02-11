using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Customers.Domain.Entities;

public sealed class CustomerAddress : Entity<Guid>
{
    public Guid CustomerId { get; private set; }
    public string Street { get; private set; } = default!;
    public string City { get; private set; } = default!;
    public string State { get; private set; } = default!;
    public string Country { get; private set; } = default!;
    public string ZipCode { get; private set; } = default!;
    public string? Label { get; private set; } // "Home", "Work", etc.

    private CustomerAddress() { }

    public static CustomerAddress Create(
        Guid customerId,
        string street,
        string city,
        string state,
        string country,
        string zipCode,
        string? label = null)
    {
        return new CustomerAddress
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            Street = street,
            City = city,
            State = state,
            Country = country,
            ZipCode = zipCode,
            Label = label,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string street, string city, string state, string country, string zipCode, string? label)
    {
        Street = street;
        City = city;
        State = state;
        Country = country;
        ZipCode = zipCode;
        Label = label;
        UpdatedAt = DateTime.UtcNow;
    }

    public string FullAddress => $"{Street}, {City}, {State} {ZipCode}, {Country}";
}
