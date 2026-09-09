using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VertexCommerce.Modules.Customers.Domain.Entities;
using VertexCommerce.Modules.Customers.Domain.ValueObjects;
using VertexCommerce.Shared.Persistence;

namespace VertexCommerce.Modules.Customers.Persistence.Configurations;

internal sealed class CustomerAddressConfiguration : IEntityTypeConfiguration<CustomerAddress>
{
    public void Configure(EntityTypeBuilder<CustomerAddress> builder)
    {
        builder.ToTable("CustomerAddresses");
        builder.HasQueryFilter(ca => !ca.IsDeleted);

        builder.HasKey(x => x.Id);

        builder.ComplexProperty(x => x.Address, address =>
        {
            address.ComplexProperty(a => a.Province, province => province
                .Property(p => p.Value)
                .HasColumnName("Province")
                .HasSchema(Province.Schema));

            address.ComplexProperty(a => a.City, city => city
                .Property(p => p.Value)
                .HasColumnName("City")
                .HasSchema(City.Schema));

            address.ComplexProperty(a => a.PostalAddress, postalAddress => postalAddress
                .Property(p => p.Value)
                .HasColumnName("PostalAddress")
                .HasSchema(PostalAddress.Schema));

            address.ComplexProperty(a => a.PostalCode, postalCode => postalCode
                .Property(p => p.Value)
                .HasColumnName("PostalCode")
                .HasSchema(PostalCode.Schema));

            address.ComplexProperty(a => a.Location, location =>
            {
                location.Property(l => l.Latitude).HasColumnName("Latitude").HasPrecision(9, 6);
                location.Property(l => l.Longitude).HasColumnName("Longitude").HasPrecision(9, 6);
            });
        });

        // Optional complex types only arrived in EF Core 10, so the one nullable value object is
        // mapped through a converter. Safe here because the label is never searched or indexed;
        // a converted column would not translate LINQ string operations.
        builder.Property(x => x.Label)
            .HasConversion(
                label => label.HasValue ? label.Value.Value : null,
                value => AddressLabel.CreateOrNull(value))
            .HasColumnName("Label")
            .HasSchema(AddressLabel.Schema);

        builder.HasIndex(x => x.CustomerId);

        // The PostalCode index is created directly in the migration; see CustomerConfiguration.
    }
}
