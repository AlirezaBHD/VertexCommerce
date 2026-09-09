using VertexCommerce.Modules.Customers.Domain.ValueObjects;
using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Customers.Domain.Entities;

/// <summary>
/// A customer's saved address. The entity exists to give the address an identity that can be
/// referenced as a default and linked from an order; the address data itself, and every rule
/// governing it, lives in <see cref="ValueObjects.Address"/>.
/// </summary>
public sealed class CustomerAddress : Entity<Guid>
{
    public Guid CustomerId { get; private set; }
    public Address Address { get; private set; } = default!;

    /// <summary>The customer's own name for this entry, such as "خانه". Absent when unnamed.</summary>
    public AddressLabel? Label { get; private set; }

    private CustomerAddress()
    {
    }

    public static CustomerAddress Create(Guid customerId, Address address, AddressLabel? label = null)
    {
        return new CustomerAddress
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            Address = address,
            Label = label,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Relocate(Address address, AddressLabel? label = null)
    {
        Address = address;
        Label = label;
        SetUpdatedAt();
    }

    public void UpdateLabel(AddressLabel? label)
    {
        Label = label;
        SetUpdatedAt();
    }
}
