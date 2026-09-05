using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VertexCommerce.Modules.Customers.Domain.Entities;

namespace VertexCommerce.Modules.Customers.Persistence.Configurations;

internal sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");
        builder.HasKey(c => c.Id);

        builder.HasIndex(c => c.UserId).IsUnique();
        builder.HasIndex(c => c.PhoneNumber);

        builder.HasMany(c => c.Addresses)
            .WithOne()
            .HasForeignKey(a => a.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
