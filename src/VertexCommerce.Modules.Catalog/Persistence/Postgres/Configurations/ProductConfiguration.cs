using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VertexCommerce.Modules.Catalog.Domain.Products;

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

        builder.Property(p => p.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasColumnName("description")
            .HasMaxLength(2000);

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

        builder.OwnsOne(p => p.Seo, seo =>
        {
            seo.Property(s => s.Slug)
                .HasColumnName("seo_slug")
                .HasMaxLength(200)
                .IsRequired();

            seo.Property(s => s.MetaTitle)
                .HasColumnName("seo_meta_title")
                .HasMaxLength(60)
                .IsRequired();

            seo.Property(s => s.MetaDescription)
                .HasColumnName("seo_meta_description")
                .HasMaxLength(160)
                .IsRequired();

            seo.Property(s => s.Keywords)
                .HasColumnName("seo_keywords")
                .HasMaxLength(500);

            seo.HasIndex(s => s.Slug).IsUnique();
        });

        builder.OwnsMany(p => p.Media, mb =>
        {
            mb.ToJson();
            mb.Property(m => m.Path).HasJsonPropertyName("media_path").IsRequired();
            mb.Property(m => m.Type).HasJsonPropertyName("media_type").IsRequired();
            mb.Property(m => m.SortOrder).HasJsonPropertyName("sort_order").IsRequired();
            mb.Property(m => m.AltText).HasJsonPropertyName("alt_text");
            mb.Property(m => m.AssociatedAttributeCode).HasJsonPropertyName("associated_attribute_code");
            mb.Property(m => m.AssociatedOptionCode).HasJsonPropertyName("associated_option_code");
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
