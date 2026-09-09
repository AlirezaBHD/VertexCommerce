using FluentAssertions;
using VertexCommerce.Modules.Customers.Domain.Entities;
using VertexCommerce.Modules.Customers.Domain.ValueObjects;
using VertexCommerce.Shared.Exceptions;
using Xunit;

namespace VertexCommerce.Modules.Customers.Tests.Domain;

public class CustomerTests
{
    private static Customer ACustomer() => Customer.Create(
        userId: Guid.NewGuid(),
        phoneNumber: PhoneNumber.Create("09123456789"),
        firstName: FirstName.Create("علی"),
        lastName: LastName.Create("احمدی"));

    private static CustomerAddress AnAddress(Guid customerId, string postalCode = "1234567890") =>
        CustomerAddress.Create(
            customerId,
            Address.Create(
                province: Province.Create("تهران"),
                city: City.Create("تهران"),
                postalAddress: PostalAddress.Create("خیابان ولیعصر"),
                postalCode: PostalCode.Create(postalCode),
                location: GeoLocation.Create(35.6892m, 51.3890m)));

    [Fact]
    public void Full_name_joins_the_two_name_parts() =>
        ACustomer().FullName.Should().Be("علی احمدی");

    [Fact]
    public void The_first_address_becomes_the_default_for_both_purposes()
    {
        var customer = ACustomer();
        var address = AnAddress(customer.Id);

        customer.AddAddress(address);

        customer.DefaultShippingAddressId.Should().Be(address.Id);
        customer.DefaultBillingAddressId.Should().Be(address.Id);
    }

    [Fact]
    public void The_address_limit_is_an_invariant_not_a_handler_check()
    {
        var customer = ACustomer();

        for (var i = 0; i < Customer.MaxAddresses; i++)
        {
            customer.AddAddress(AnAddress(customer.Id));
        }

        customer.CanAddAddress.Should().BeFalse();

        var act = () => customer.AddAddress(AnAddress(customer.Id));
        act.Should().Throw<BusinessRuleException>()
            .Which.ErrorCode.Should().Be("Customer.TooManyAddresses");
    }

    [Fact]
    public void Removing_the_default_address_promotes_a_remaining_one()
    {
        var customer = ACustomer();
        var first = AnAddress(customer.Id);
        var second = AnAddress(customer.Id, "9876543210");

        customer.AddAddress(first);
        customer.AddAddress(second);

        customer.RemoveAddress(first.Id);

        customer.DefaultShippingAddressId.Should().Be(second.Id);
        customer.DefaultBillingAddressId.Should().Be(second.Id);
    }

    [Fact]
    public void An_address_the_customer_does_not_own_cannot_become_their_default()
    {
        var customer = ACustomer();
        customer.AddAddress(AnAddress(customer.Id));

        var act = () => customer.SetDefaultShippingAddress(Guid.NewGuid());

        act.Should().Throw<BusinessRuleException>()
            .Which.ErrorCode.Should().Be("Customer.AddressNotOwned");
    }

    [Fact]
    public void Updating_the_profile_replaces_every_part_and_stamps_the_change()
    {
        var customer = ACustomer();

        customer.UpdateProfile(
            PhoneNumber.Create("09350001122"),
            FirstName.Create("رضا"),
            LastName.Create("کریمی"));

        customer.PhoneNumber.Should().Be(PhoneNumber.Create("09350001122"));
        customer.FullName.Should().Be("رضا کریمی");
        customer.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Two_addresses_describing_the_same_place_are_equal_regardless_of_label()
    {
        var customer = ACustomer();

        var home = AnAddress(customer.Id);
        var work = AnAddress(customer.Id);

        home.Address.Should().Be(work.Address,
            "a label names the saved entry, it is not part of the address itself");
    }
}
