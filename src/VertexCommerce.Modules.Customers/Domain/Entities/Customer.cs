using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Customers.Domain.Entities;

public sealed class Customer : AggregateRoot<Guid>
{
    public Guid? UserId { get; private set; }
    public string PhoneNumber { get; private set; } = default!;
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;

    private readonly List<CustomerAddress> _addresses = [];
    public IReadOnlyCollection<CustomerAddress> Addresses => _addresses.AsReadOnly();

    public Guid? DefaultShippingAddressId { get; private set; }
    public Guid? DefaultBillingAddressId { get; private set; }

    private Customer()
    {
    }

    public static Customer Create(Guid? userId, string phoneNumber, string firstName, string lastName)
    {
        return new Customer
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            PhoneNumber = phoneNumber,
            FirstName = firstName,
            LastName = lastName,
            CreatedAt = DateTime.UtcNow
        };
    }

    public string FullName => $"{FirstName} {LastName}";

    public void UpdateProfile(string phoneNumber, string firstName, string lastName)
    {
        PhoneNumber = phoneNumber;
        FirstName = firstName;
        LastName = lastName;
        SetUpdatedAt();
    }

    public void AddAddress(
        CustomerAddress address)
    {
        _addresses.Add(address);

        // First address becomes default
        if (_addresses.Count == 1)
        {
            DefaultShippingAddressId = address.Id;
            DefaultBillingAddressId = address.Id;
        }

        SetUpdatedAt();
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

        SetUpdatedAt();
    }

    public void SetDefaultShippingAddress(Guid addressId)
    {
        if (!_addresses.Any(a => a.Id == addressId))
            throw new InvalidOperationException("Address not found");

        DefaultShippingAddressId = addressId;
        SetUpdatedAt();
    }

    public void SetDefaultBillingAddress(Guid addressId)
    {
        if (!_addresses.Any(a => a.Id == addressId))
            throw new InvalidOperationException("Address not found");

        DefaultBillingAddressId = addressId;
        SetUpdatedAt();
    }

    public CustomerAddress? GetDefaultShippingAddress()
        => _addresses.FirstOrDefault(a => a.Id == DefaultShippingAddressId);

    public CustomerAddress? GetDefaultBillingAddress()
        => _addresses.FirstOrDefault(a => a.Id == DefaultBillingAddressId);
}
