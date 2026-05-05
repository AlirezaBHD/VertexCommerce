using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VertexCommerce.Modules.Catalog.Domain.Products;

namespace VertexCommerce.Modules.Catalog.Persistence.Postgres.Configurations;

public sealed class CatalogAttributeOptionConfiguration : IEntityTypeConfiguration<CatalogAttributeOption>
{
    public void Configure(EntityTypeBuilder<CatalogAttributeOption> builder)
    {
        builder.ToTable("catalog_attribute_options", "catalog");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(a => a.AttributeId)
            .HasColumnName("attribute_id")
            .IsRequired();

        builder.Property(a => a.Code)
            .HasColumnName("code")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.DefaultName)
            .HasColumnName("default_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.MediaPath)
            .HasColumnName("media_path")
            .HasMaxLength(100);

        builder.Property(a => a.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(a => a.UpdatedAt)
            .HasColumnName("updated_at");

        builder.HasIndex(a => new { a.AttributeId, a.Code }).IsUnique();
    }
}
