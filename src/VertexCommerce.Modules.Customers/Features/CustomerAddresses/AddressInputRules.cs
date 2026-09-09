using FluentValidation;
using VertexCommerce.Modules.Customers.Domain.ValueObjects;

namespace VertexCommerce.Modules.Customers.Features.CustomerAddresses;

/// <summary>
/// The boundary validation for <see cref="IAddressInput"/>, sourced from the same specs that guard
/// the value objects. Rejecting input here yields a structured 400 naming every offending field,
/// rather than letting the domain guard surface a single exception.
/// </summary>
internal static class AddressInputRules
{
    public static void ApplyAddressRules<TCommand>(this AbstractValidator<TCommand> validator)
        where TCommand : IAddressInput
    {
        ArgumentNullException.ThrowIfNull(validator);

        validator.RuleFor(x => x.Province).MustInstantiate(Province.Create);
        validator.RuleFor(x => x.City).MustInstantiate(City.Create);
        validator.RuleFor(x => x.PostalAddress).MustInstantiate(PostalAddress.Create);
        validator.RuleFor(x => x.PostalCode).MustInstantiate(PostalCode.Create);
        validator.RuleFor(x => x.Label).MustInstantiate(AddressLabel.CreateOrNull);

        validator.RuleFor(x => x.Latitude)
            .InclusiveBetween(GeoLocation.MinLatitude, GeoLocation.MaxLatitude);

        validator.RuleFor(x => x.Longitude)
            .InclusiveBetween(GeoLocation.MinLongitude, GeoLocation.MaxLongitude);
    }
}
