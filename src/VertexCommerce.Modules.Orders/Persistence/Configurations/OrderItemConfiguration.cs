using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VertexCommerce.Modules.Orders.Domain.Entities;

namespace VertexCommerce.Modules.Orders.Persistence.Configurations;

public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(i => i.OrderId)
            .HasColumnName("order_id")
            .IsRequired();

        builder.Property(i => i.ProductId)
            .HasColumnName("product_id")
            .IsRequired();

        builder.Property(i => i.ProductName)
            .HasColumnName("product_name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(i => i.ProductSku)
            .HasColumnName("product_sku")
            .HasMaxLength(50);

        // Value Object: UnitPrice
        builder.OwnsOne(i => i.UnitPrice, money =>
        {
            money.Property(m => m.Amount).HasColumnName("unit_price_amount").HasPrecision(18, 2).IsRequired();
            money.Property(m => m.Currency).HasColumnName("unit_price_currency").HasMaxLength(3).IsRequired();
        });

        builder.Property(i => i.Quantity)
            .HasColumnName("quantity")
            .IsRequired();

        builder.Property(i => i.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(i => i.UpdatedAt)
            .HasColumnName("updated_at");

        // Ignore computed property
        builder.Ignore(i => i.TotalPrice);

        // Indexes
        builder.HasIndex(i => i.OrderId);
        builder.HasIndex(i => i.ProductId);
    }
}
