using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VertexCommerce.Modules.Orders.Domain.Entities;

namespace VertexCommerce.Modules.Orders.Persistence.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(o => o.OrderNumber)
            .HasColumnName("order_number")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(o => o.CustomerId)
            .HasColumnName("customer_id")
            .IsRequired();

        builder.Property(o => o.CustomerEmail)
            .HasColumnName("customer_email")
            .HasMaxLength(256);

        builder.Property(o => o.Status)
            .HasColumnName("status")
            .IsRequired();

        builder.Property(o => o.PaymentStatus)
            .HasColumnName("payment_status")
            .IsRequired();

        // Value Object: ShippingAddress
        builder.OwnsOne(o => o.ShippingAddress, address =>
        {
            address.Property(a => a.Street).HasColumnName("shipping_street").HasMaxLength(200).IsRequired();
            address.Property(a => a.City).HasColumnName("shipping_city").HasMaxLength(100).IsRequired();
            address.Property(a => a.State).HasColumnName("shipping_state").HasMaxLength(100);
            address.Property(a => a.Country).HasColumnName("shipping_country").HasMaxLength(100).IsRequired();
            address.Property(a => a.ZipCode).HasColumnName("shipping_zip_code").HasMaxLength(20).IsRequired();
        });

        // Value Object: BillingAddress
        builder.OwnsOne(o => o.BillingAddress, address =>
        {
            address.Property(a => a.Street).HasColumnName("billing_street").HasMaxLength(200);
            address.Property(a => a.City).HasColumnName("billing_city").HasMaxLength(100);
            address.Property(a => a.State).HasColumnName("billing_state").HasMaxLength(100);
            address.Property(a => a.Country).HasColumnName("billing_country").HasMaxLength(100);
            address.Property(a => a.ZipCode).HasColumnName("billing_zip_code").HasMaxLength(20);
        });

        // Value Object: SubTotal
        builder.OwnsOne(o => o.SubTotal, money =>
        {
            money.Property(m => m.Amount).HasColumnName("sub_total_amount").HasPrecision(18, 2).IsRequired();
            money.Property(m => m.Currency).HasColumnName("sub_total_currency").HasMaxLength(3).IsRequired();
        });

        // Value Object: ShippingCost
        builder.OwnsOne(o => o.ShippingCost, money =>
        {
            money.Property(m => m.Amount).HasColumnName("shipping_cost_amount").HasPrecision(18, 2).IsRequired();
            money.Property(m => m.Currency).HasColumnName("shipping_cost_currency").HasMaxLength(3).IsRequired();
        });

        // Value Object: Tax
        builder.OwnsOne(o => o.Tax, money =>
        {
            money.Property(m => m.Amount).HasColumnName("tax_amount").HasPrecision(18, 2).IsRequired();
            money.Property(m => m.Currency).HasColumnName("tax_currency").HasMaxLength(3).IsRequired();
        });

        // Value Object: TotalAmount
        builder.OwnsOne(o => o.TotalAmount, money =>
        {
            money.Property(m => m.Amount).HasColumnName("total_amount").HasPrecision(18, 2).IsRequired();
            money.Property(m => m.Currency).HasColumnName("total_currency").HasMaxLength(3).IsRequired();
        });

        builder.Property(o => o.Notes)
            .HasColumnName("notes")
            .HasMaxLength(1000);

        builder.Property(o => o.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(o => o.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(o => o.ShippedAt)
            .HasColumnName("shipped_at");

        builder.Property(o => o.DeliveredAt)
            .HasColumnName("delivered_at");

        builder.Property(o => o.CancelledAt)
            .HasColumnName("cancelled_at");

        builder.Property(o => o.CancellationReason)
            .HasColumnName("cancellation_reason")
            .HasMaxLength(500);

        // Relationships
        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore domain events
        builder.Ignore(o => o.DomainEvents);

        // Indexes
        builder.HasIndex(o => o.OrderNumber).IsUnique();
        builder.HasIndex(o => o.CustomerId);
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.CreatedAt);
    }
}