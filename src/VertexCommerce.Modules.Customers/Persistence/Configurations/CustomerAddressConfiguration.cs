using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VertexCommerce.Modules.Customers.Domain.Entities;

namespace VertexCommerce.Modules.Customers.Persistence.Configurations;

internal sealed class CustomerAddressConfiguration : IEntityTypeConfiguration<CustomerAddress>
{
    public void Configure(EntityTypeBuilder<CustomerAddress> builder)
    {
        builder.ToTable("CustomerAddresses");
        builder.HasQueryFilter(ca => !ca.IsDeleted);
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Province).HasMaxLength(100).IsRequired();
        builder.Property(x => x.City).HasMaxLength(100).IsRequired();
        builder.Property(x => x.PostalAddress).HasMaxLength(500).IsRequired();
        builder.Property(x => x.PostalCode).HasMaxLength(10).IsRequired();
        builder.Property(x => x.Latitude).HasPrecision(9, 6).IsRequired();
        builder.Property(x => x.Longitude).HasPrecision(9, 6).IsRequired();
        builder.Property(x => x.Label).HasMaxLength(50);

        builder.HasIndex(x => x.CustomerId);
        builder.HasIndex(x => x.PostalCode);}
}
