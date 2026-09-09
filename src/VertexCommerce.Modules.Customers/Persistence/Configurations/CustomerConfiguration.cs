using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VertexCommerce.Modules.Customers.Domain.Entities;
using VertexCommerce.Modules.Customers.Domain.ValueObjects;
using VertexCommerce.Shared.Persistence;

namespace VertexCommerce.Modules.Customers.Persistence.Configurations;

internal sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");
        builder.HasKey(c => c.Id);

        // Each value object is flattened onto the column it has always occupied, so introducing
        // them is a no-op for the schema. Lengths come from the spec that also guards the domain.
        builder.ComplexProperty(c => c.PhoneNumber, phone => phone
            .Property(p => p.Value)
            .HasColumnName("PhoneNumber")
            .HasSchema(PhoneNumber.Schema));
        

        builder.ComplexProperty(c => c.FirstName, firstName => firstName
            .Property(p => p.Value)
            .HasColumnName("FirstName")
            .HasSchema(FirstName.Schema));

        builder.ComplexProperty(c => c.LastName, lastName => lastName
            .Property(p => p.Value)
            .HasColumnName("LastName")
            .HasSchema(LastName.Schema));

        builder.HasIndex(c => c.UserId).IsUnique();

        // The PhoneNumber index is created directly in the migration: EF cannot declare an index
        // over a complex type's property until EF Core 11 (dotnet/efcore#31246). Its presence is
        // asserted by an integration test so it cannot silently disappear.

        builder.HasMany(c => c.Addresses)
            .WithOne()
            .HasForeignKey(a => a.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
