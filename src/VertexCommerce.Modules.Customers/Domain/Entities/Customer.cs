using VertexCommerce.Modules.Customers.Domain.ValueObjects;
using VertexCommerce.Shared.Domain;
using VertexCommerce.Shared.Exceptions;

namespace VertexCommerce.Modules.Customers.Domain.Entities;

public sealed class Customer : AggregateRoot<Guid>
{
    public const int MaxAddresses = 3;

    public Guid? UserId { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public FirstName FirstName { get; private set; }
    public LastName LastName { get; private set; }

    private readonly List<CustomerAddress> _addresses = [];
    public IReadOnlyCollection<CustomerAddress> Addresses => _addresses;

    public Guid? DefaultShippingAddressId { get; private set; }
    public Guid? DefaultBillingAddressId { get; private set; }

    public bool CanAddAddress => 
        !_addresses.Where(a => !a.IsDeleted)
            .Skip(MaxAddresses - 1)
            .Any();

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
        
        var activeAddresses = _addresses.Where(a => !a.IsDeleted);
        var hasExactlyOneActiveAddress =
            activeAddresses.Take(2).Count() == 1;
        
        // First address becomes default
        if (DefaultShippingAddressId is null || hasExactlyOneActiveAddress)
        {
            DefaultShippingAddressId = address.Id;
        }

        if (DefaultBillingAddressId is null || hasExactlyOneActiveAddress)
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
        if (!_addresses.Exists(a => a.Id == addressId && !a.IsDeleted))
            throw new BusinessRuleException("Customer.AddressNotOwned", "Address not found or does not belong to customer.");

        DefaultShippingAddressId = addressId;
        SetUpdatedAt();
    }

    public void SetDefaultBillingAddress(Guid addressId)
    {
        if (!_addresses.Exists(a => a.Id == addressId && !a.IsDeleted))
            throw new BusinessRuleException("Customer.AddressNotOwned", "Address not found or does not belong to customer.");

        DefaultBillingAddressId = addressId;
        SetUpdatedAt();
    }

    public CustomerAddress? GetDefaultShippingAddress()
        => _addresses.FirstOrDefault(a => a.Id == DefaultShippingAddressId && !a.IsDeleted);

    public CustomerAddress? GetDefaultBillingAddress()
        => _addresses.FirstOrDefault(a => a.Id == DefaultBillingAddressId && !a.IsDeleted);
}
