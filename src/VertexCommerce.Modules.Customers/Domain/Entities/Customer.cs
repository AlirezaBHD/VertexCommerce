using VertexCommerce.Modules.Customers.Domain.ValueObjects;
using VertexCommerce.Shared.Domain;
using VertexCommerce.Shared.Domain.Schema;
using VertexCommerce.Shared.Exceptions;

namespace VertexCommerce.Modules.Customers.Domain.Entities;

public sealed class Customer : AggregateRoot<Guid>
{
    public const int MaxAddresses = 3;

    public Guid? UserId { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; } = default!;
    public FirstName FirstName { get; private set; } = default!;
    public LastName LastName { get; private set; } = default!;

    private readonly List<CustomerAddress> _addresses = [];
    public IReadOnlyCollection<CustomerAddress> Addresses => _addresses;

    public Guid? DefaultShippingAddressId { get; private set; }
    public Guid? DefaultBillingAddressId { get; private set; }

    public bool CanAddAddress => _addresses.Count(a => !a.IsDeleted) < MaxAddresses;

    private Customer()
    {
    }

    public static Customer Create(Guid? userId, PhoneNumber phoneNumber, FirstName firstName, LastName lastName)
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

    public string FullName => $"{FirstName.Value} {LastName.Value}";

    public void UpdateProfile(PhoneNumber phoneNumber, FirstName firstName, LastName lastName)
    {
        PhoneNumber = phoneNumber;
        FirstName = firstName;
        LastName = lastName;
        SetUpdatedAt();
    }

    public void AddAddress(CustomerAddress address)
    {
        ArgumentNullException.ThrowIfNull(address);

        if (!CanAddAddress)
        {
            throw new BusinessRuleException(
                "Customer.TooManyAddresses",
                $"A customer cannot have more than {MaxAddresses} addresses.");
        }

        _addresses.Add(address);

        // First address becomes default
        if (DefaultShippingAddressId is null || _addresses.Count(a => !a.IsDeleted) == 1)
        {
            DefaultShippingAddressId = address.Id;
        }

        if (DefaultBillingAddressId is null || _addresses.Count(a => !a.IsDeleted) == 1)
        {
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
            DefaultShippingAddressId = _addresses.FirstOrDefault(a => !a.IsDeleted)?.Id;

        if (DefaultBillingAddressId == addressId)
            DefaultBillingAddressId = _addresses.FirstOrDefault(a => !a.IsDeleted)?.Id;

        SetUpdatedAt();
    }

    public CustomerAddress? FindAddress(Guid addressId)
        => _addresses.FirstOrDefault(a => a.Id == addressId && !a.IsDeleted);

    public void SetDefaultShippingAddress(Guid addressId)
    {
        if (!_addresses.Any(a => a.Id == addressId && !a.IsDeleted))
            throw new BusinessRuleException("Customer.AddressNotOwned", "Address not found or does not belong to customer.");

        DefaultShippingAddressId = addressId;
        SetUpdatedAt();
    }

    public void SetDefaultBillingAddress(Guid addressId)
    {
        if (!_addresses.Any(a => a.Id == addressId && !a.IsDeleted))
            throw new BusinessRuleException("Customer.AddressNotOwned", "Address not found or does not belong to customer.");

        DefaultBillingAddressId = addressId;
        SetUpdatedAt();
    }

    public CustomerAddress? GetDefaultShippingAddress()
        => _addresses.FirstOrDefault(a => a.Id == DefaultShippingAddressId && !a.IsDeleted);

    public CustomerAddress? GetDefaultBillingAddress()
        => _addresses.FirstOrDefault(a => a.Id == DefaultBillingAddressId && !a.IsDeleted);
}
