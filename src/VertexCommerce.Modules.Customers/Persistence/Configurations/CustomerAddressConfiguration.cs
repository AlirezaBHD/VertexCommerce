using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VertexCommerce.Modules.Customers.Domain.Entities;

namespace VertexCommerce.Modules.Customers.Persistence.Configurations;

internal sealed class CustomerAddressConfiguration : IEntityTypeConfiguration<CustomerAddress>
{
    public void Configure(EntityTypeBuilder<CustomerAddress> builder)
    {
        builder.ToTable("CustomerAddresses");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Street).HasMaxLength(200).IsRequired();
        builder.Property(a => a.City).HasMaxLength(100).IsRequired();
        builder.Property(a => a.State).HasMaxLength(100).IsRequired();
        builder.Property(a => a.Country).HasMaxLength(100).IsRequired();
        builder.Property(a => a.ZipCode).HasMaxLength(20).IsRequired();
        builder.Property(a => a.Label).HasMaxLength(50);
    }
}
