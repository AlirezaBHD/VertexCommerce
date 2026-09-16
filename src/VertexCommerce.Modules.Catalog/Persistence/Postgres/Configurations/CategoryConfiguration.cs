using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VertexCommerce.Modules.Catalog.Domain.Categories;
using VertexCommerce.Modules.Catalog.Domain.ValueObjects;
using VertexCommerce.Shared.Persistence;

namespace VertexCommerce.Modules.Catalog.Persistence.Postgres.Configurations;

public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories", "catalog");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.ComplexProperty(c => c.Name, name => name.Property(x => x.Value).HasColumnName("name").HasSchema(CategoryName.Schema).IsRequired());

        builder.ComplexProperty(c => c.Description, desc => desc.Property(x => x.Value).HasColumnName("description").HasSchema(CategoryDescription.Schema));
        
        builder.Property(c => c.IconPath).HasConversion(x => x.HasValue ? x.Value.Value : null, v => string.IsNullOrEmpty(v) ? null : ImagePath.CreateOrNull(v)).HasColumnName("icon_path").HasSchema(ImagePath.Schema);
        
        builder.ComplexProperty(c => c.CoverImagePath, p => p.Property(x => x.Value).HasColumnName("cover_image_path").HasSchema(ImagePath.Schema).IsRequired());

        builder.Property(c => c.ImageAltText).HasConversion(x => x.HasValue ? x.Value.Value : null, v => string.IsNullOrEmpty(v) ? null : AltText.CreateOrNull(v)).HasColumnName("image_alt_text").HasSchema(AltText.Schema);

        builder.Property(c => c.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.Property(c => c.ShowOnHome)
            .HasColumnName("show_on_home")
            .IsRequired();

        builder.Property(c => c.IncludeInMenu)
            .HasColumnName("include_in_menu")
            .IsRequired();

        builder.Property(c => c.ParentId)
            .HasColumnName("parent_id");
        
        builder.Property(c => c.SortOrder)
            .HasColumnName("sort_order")
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .HasColumnName("updated_at");

        builder.HasOne<Category>()
            .WithMany(c => c.Children)
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ComplexProperty(c => c.Seo, seo =>
        {
            seo.Property(s => s.Slug).HasConversion(x => x.Value, v => Slug.Create(v))
                .HasColumnName("seo_slug")
                .HasMaxLength(200)
                .IsRequired();

            seo.Property(s => s.MetaTitle).HasConversion(x => x.HasValue ? x.Value.Value : null, v => string.IsNullOrEmpty(v) ? null : MetaTitle.CreateOrNull(v))
                .HasColumnName("seo_meta_title")
                .HasMaxLength(60)
                .IsRequired();

            seo.Property(s => s.MetaDescription).HasConversion(x => x.HasValue ? x.Value.Value : null, v => string.IsNullOrEmpty(v) ? null : MetaDescription.CreateOrNull(v))
                .HasColumnName("seo_meta_description")
                .HasMaxLength(160)
                .IsRequired();

            seo.Property(s => s.Keywords).HasConversion(x => x.HasValue ? x.Value.Value : null, v => string.IsNullOrEmpty(v) ? null : SeoKeywords.CreateOrNull(v))
                .HasColumnName("seo_keywords")
                .HasMaxLength(500);

        });
        
        builder.Ignore(c => c.DomainEvents);
        builder.HasIndex(c => c.ParentId);
        builder.HasIndex(c => c.IsActive);
    }
}
