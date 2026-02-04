using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VertexCommerce.Modules.Catalog.Domain.Entities;

namespace VertexCommerce.Modules.Catalog.Persistence.Configurations;

public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");

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

        builder.Property(c => c.ParentId)
            .HasColumnName("parent_id");

        builder.Property(c => c.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

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

        builder.Ignore(c => c.DomainEvents);

        builder.HasIndex(c => c.Name);
        builder.HasIndex(c => c.ParentId);
        builder.HasIndex(c => c.IsActive);
    }
}
