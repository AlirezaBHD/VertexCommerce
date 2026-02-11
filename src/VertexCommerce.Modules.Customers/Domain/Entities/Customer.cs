using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Customers.Domain.Entities;

public sealed class Customer : AggregateRoot<Guid>
{
    public Guid UserId { get; private set; } // Link to Identity
    public string Email { get; private set; } = default!;
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string? Phone { get; private set; }

    private readonly List<CustomerAddress> _addresses = [];
    public IReadOnlyCollection<CustomerAddress> Addresses => _addresses.AsReadOnly();

    public Guid? DefaultShippingAddressId { get; private set; }
    public Guid? DefaultBillingAddressId { get; private set; }

    private Customer() { }

    public static Customer Create(Guid userId, string email, string firstName, string lastName)
    {
        return new Customer
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Email = email.ToLowerInvariant(),
            FirstName = firstName,
            LastName = lastName,
            CreatedAt = DateTime.UtcNow
        };
    }

    public string FullName => $"{FirstName} {LastName}";

    public void UpdateProfile(string firstName, string lastName, string? phone)
    {
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        UpdatedAt = DateTime.UtcNow;
    }

    public CustomerAddress AddAddress(
        string street,
        string city,
        string state,
        string country,
        string zipCode,
        string? label = null)
    {
        var address = CustomerAddress.Create(Id, street, city, state, country, zipCode, label);
        _addresses.Add(address);

        // First address becomes default
        if (_addresses.Count == 1)
        {
            DefaultShippingAddressId = address.Id;
            DefaultBillingAddressId = address.Id;
        }

        UpdatedAt = DateTime.UtcNow;
        return address;
    }

    public void RemoveAddress(Guid addressId)
    {
        var address = _addresses.FirstOrDefault(a => a.Id == addressId);
        if (address is null) return;

        _addresses.Remove(address);

        if (DefaultShippingAddressId == addressId)
            DefaultShippingAddressId = _addresses.FirstOrDefault()?.Id;

        if (DefaultBillingAddressId == addressId)
            DefaultBillingAddressId = _addresses.FirstOrDefault()?.Id;

        UpdatedAt = DateTime.UtcNow;
    }

    public void SetDefaultShippingAddress(Guid addressId)
    {
        if (!_addresses.Any(a => a.Id == addressId))
            throw new InvalidOperationException("Address not found");

        DefaultShippingAddressId = addressId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetDefaultBillingAddress(Guid addressId)
    {
        if (!_addresses.Any(a => a.Id == addressId))
            throw new InvalidOperationException("Address not found");

        DefaultBillingAddressId = addressId;
        UpdatedAt = DateTime.UtcNow;
    }

    public CustomerAddress? GetDefaultShippingAddress()
        => _addresses.FirstOrDefault(a => a.Id == DefaultShippingAddressId);

    public CustomerAddress? GetDefaultBillingAddress()
        => _addresses.FirstOrDefault(a => a.Id == DefaultBillingAddressId);
}