using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VertexCommerce.Modules.Catalog.Domain.Products;

namespace VertexCommerce.Modules.Catalog.Persistence.Postgres.Configurations;

public sealed class CatalogAttributeConfiguration : IEntityTypeConfiguration<CatalogAttribute>
{
    public void Configure(EntityTypeBuilder<CatalogAttribute> builder)
    {
        builder.ToTable("catalog_attributes", "catalog");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(a => a.Code)
            .HasColumnName("code")
            .HasMaxLength(100)
            .IsRequired();
        
        builder.HasMany(a => a.Options)
            .WithOne()
            .HasForeignKey(o => o.AttributeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(a => a.DefaultName)
            .HasColumnName("default_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.Type)
            .HasColumnName("type")
            .HasMaxLength(50);

        builder.Property(a => a.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(a => a.UpdatedAt)
            .HasColumnName("updated_at");

        builder.HasIndex(a => a.Code).IsUnique();
    }
}
