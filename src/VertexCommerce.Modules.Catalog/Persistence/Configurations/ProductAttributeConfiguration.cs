using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VertexCommerce.Modules.Catalog.Domain.Entities;

namespace VertexCommerce.Modules.Catalog.Persistence.Configurations;

public sealed class ProductAttributeConfiguration : IEntityTypeConfiguration<ProductAttribute>
{
    public void Configure(EntityTypeBuilder<ProductAttribute> builder)
    {
        builder.ToTable("product_attributes");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(a => a.ProductId)
            .HasColumnName("product_id")
            .IsRequired();

        builder.Property(a => a.Key)
            .HasColumnName("key")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.Value)
            .HasColumnName("value")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(a => a.Type)
            .HasColumnName("type")
            .HasMaxLength(50);

        builder.Property(a => a.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(a => a.UpdatedAt)
            .HasColumnName("updated_at");

        builder.HasIndex(a => new { a.ProductId, a.Key }).IsUnique();
    }
}
