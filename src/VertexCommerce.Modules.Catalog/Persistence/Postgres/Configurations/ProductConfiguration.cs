using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VertexCommerce.Modules.Catalog.Domain.Products;
using VertexCommerce.Modules.Catalog.Domain.ValueObjects;
using VertexCommerce.Shared.Persistence;

namespace VertexCommerce.Modules.Catalog.Persistence.Postgres.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products", "catalog");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.ComplexProperty(p => p.Name, name => name.Property(x => x.Value).HasColumnName("name").HasSchema(ProductName.Schema).IsRequired());

        builder.Property(p => p.Description).HasConversion(x => x.HasValue ? x.Value.Value : null, v => string.IsNullOrEmpty(v) ? null : ProductDescription.Create(v)).HasColumnName("description").HasSchema(ProductDescription.Schema);

        builder.Property(p => p.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.Property(p => p.CategoryId)
            .HasColumnName("category_id")
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at");

        builder.ComplexProperty(p => p.Seo, seo =>
        {
            seo.ComplexProperty(s => s.Slug, slug => slug.Property(x => x.Value).HasColumnName("slug").HasSchema(Slug.Schema).IsRequired());
            seo.Property(s => s.MetaTitle).HasConversion(x => x.HasValue ? x.Value.Value : null, v => string.IsNullOrEmpty(v) ? null : MetaTitle.CreateOrNull(v)).HasColumnName("meta_title").HasSchema(MetaTitle.Schema);
            seo.Property(s => s.MetaDescription).HasConversion(x => x.HasValue ? x.Value.Value : null, v => string.IsNullOrEmpty(v) ? null : MetaDescription.CreateOrNull(v)).HasColumnName("meta_description").HasSchema(MetaDescription.Schema);
            seo.Property(s => s.Keywords).HasConversion(x => x.HasValue ? x.Value.Value : null, v => string.IsNullOrEmpty(v) ? null : SeoKeywords.CreateOrNull(v)).HasColumnName("keywords").HasSchema(SeoKeywords.Schema);
        });

        builder.OwnsMany(p => p.Media, mb =>
        {
            mb.ToJson();
            mb.Property(m => m.Path).HasConversion(x => x.Value, v => ImagePath.Create(v)).HasJsonPropertyName("media_path").IsRequired();
            mb.Property(m => m.Type).HasJsonPropertyName("media_type").IsRequired();
            mb.Property(m => m.SortOrder).HasJsonPropertyName("sort_order").IsRequired();
            mb.Property(m => m.AltText).HasConversion(x => x.HasValue ? x.Value.Value : null, v => string.IsNullOrEmpty(v) ? null : AltText.CreateOrNull(v)).HasJsonPropertyName("alt_text");
            mb.Property(m => m.AssociatedAttributeCode).HasConversion(x => x.HasValue ? x.Value.Value : null, v => string.IsNullOrEmpty(v) ? null : AttributeCode.Create(v)).HasJsonPropertyName("associated_attribute_code");
            mb.Property(m => m.AssociatedOptionCode).HasConversion(x => x.HasValue ? x.Value.Value : null, v => string.IsNullOrEmpty(v) ? null : OptionCode.Create(v)).HasJsonPropertyName("associated_option_code");
        });

        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(p => p.DomainEvents);

        builder.HasIndex(p => p.CategoryId);
        builder.HasIndex(p => p.IsActive);

        builder.HasMany(p => p.Variants)
            .WithOne()
            .HasForeignKey(v => v.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
