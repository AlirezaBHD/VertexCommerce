using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VertexCommerce.Modules.Catalog.Domain.Categories;

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

        builder.Property(c => c.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasColumnName("description")
            .HasMaxLength(500);
        
        builder.Property(c => c.IconPath)
            .HasColumnName("icon_path")
            .HasMaxLength(255);
        
        builder.Property(c => c.CoverImagePath)
            .HasColumnName("cover_image_path")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(c => c.ImageAltText)
            .HasColumnName("image_alt_text")
            .HasMaxLength(200);

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

        });
        
        builder.Ignore(c => c.DomainEvents);
        builder.HasIndex(c => c.ParentId);
        builder.HasIndex(c => c.IsActive);
    }
}
