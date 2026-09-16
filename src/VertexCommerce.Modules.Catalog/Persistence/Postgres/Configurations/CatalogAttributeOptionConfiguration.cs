using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VertexCommerce.Modules.Catalog.Domain.Products;
using VertexCommerce.Modules.Catalog.Domain.ValueObjects;
using VertexCommerce.Shared.Persistence;

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

        builder.Property(a => a.Code).HasConversion(x => x.Value, v => OptionCode.Create(v)).HasColumnName("code").HasSchema(OptionCode.Schema).IsRequired();

        builder.ComplexProperty(a => a.DefaultName, name => name.Property(x => x.Value).HasColumnName("default_name").HasSchema(OptionValue.Schema).IsRequired());

        builder.Property(a => a.MediaPath).HasConversion(x => x.HasValue ? x.Value.Value : null, v => string.IsNullOrEmpty(v) ? null : ImagePath.CreateOrNull(v)).HasColumnName("media_path").HasSchema(ImagePath.Schema);

        builder.Property(a => a.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(a => a.UpdatedAt)
            .HasColumnName("updated_at");

        builder.HasIndex(a => new { a.AttributeId, a.Code }).IsUnique();
    }
}
