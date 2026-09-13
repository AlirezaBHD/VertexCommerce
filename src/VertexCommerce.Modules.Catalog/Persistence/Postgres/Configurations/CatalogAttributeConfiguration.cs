using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VertexCommerce.Modules.Catalog.Domain.Products;
using VertexCommerce.Modules.Catalog.Domain.ValueObjects;
using VertexCommerce.Shared.Persistence;

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

        builder.ComplexProperty(a => a.Code, code => code.Property(x => x.Value).HasColumnName("code").HasSchema(AttributeCode.Schema).IsRequired());
        
        builder.HasMany(a => a.Options)
            .WithOne()
            .HasForeignKey(o => o.AttributeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ComplexProperty(a => a.DefaultName, name => name.Property(x => x.Value).HasColumnName("default_name").HasSchema(AttributeName.Schema).IsRequired());

        builder.Property(a => a.Type).HasConversion(x => x.HasValue ? x.Value.Value : null, v => string.IsNullOrEmpty(v) ? null : AttributeType.CreateOrNull(v)).HasColumnName("type").HasSchema(AttributeType.Schema);

        builder.Property(a => a.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(a => a.UpdatedAt)
            .HasColumnName("updated_at");

        builder.HasIndex(a => a.Code).IsUnique();
    }
}
