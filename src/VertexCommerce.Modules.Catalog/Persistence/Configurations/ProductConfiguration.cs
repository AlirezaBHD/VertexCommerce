using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VertexCommerce.Modules.Catalog.Domain.Entities;

namespace VertexCommerce.Modules.Catalog.Persistence.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

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

        builder.OwnsOne(p => p.Sku, sku =>
        {
            sku.Property(s => s.Value)
                .HasColumnName("sku")
                .HasMaxLength(50)
                .IsRequired();

            sku.HasIndex(s => s.Value).IsUnique();
        });

        builder.OwnsOne(p => p.Price, price =>
        {
            price.Property(m => m.Amount)
                .HasColumnName("price_amount")
                .HasPrecision(18, 2)
                .IsRequired();

            price.Property(m => m.Currency)
                .HasColumnName("price_currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(p => p.StockQuantity)
            .HasColumnName("stock_quantity")
            .IsRequired();

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

        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Attributes)
            .WithOne()
            .HasForeignKey(a => a.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(p => p.DomainEvents);

        builder.HasIndex(p => p.Name);
        builder.HasIndex(p => p.CategoryId);
        builder.HasIndex(p => p.IsActive);
    }
}