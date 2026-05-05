using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VertexCommerce.Modules.Orders.Domain.Entities;

namespace VertexCommerce.Modules.Orders.Persistence.Configurations;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        builder.HasKey(o => o.Id);

        builder.Property(o => o.OrderNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(o => o.OrderNumber).IsUnique();
        builder.HasIndex(o => o.CustomerId);

        builder.Property(o => o.CustomerPhoneNumber)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(o => o.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(o => o.PaymentStatus)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(o => o.Notes).HasMaxLength(1000);
        builder.Property(o => o.CancellationReason).HasMaxLength(500);
        builder.Property(o => o.TrackingNumber).HasMaxLength(100);

        builder.OwnsOne(o => o.SubTotal, money =>
        {
            money.Property(m => m.Amount).HasColumnName("SubTotal").HasPrecision(18, 2);
            money.Property(m => m.Currency).HasColumnName("SubTotalCurrency").HasMaxLength(3);
        });

        builder.OwnsOne(o => o.ShippingCost, money =>
        {
            money.Property(m => m.Amount).HasColumnName("ShippingCost").HasPrecision(18, 2);
            money.Property(m => m.Currency).HasColumnName("ShippingCostCurrency").HasMaxLength(3);
        });

        builder.OwnsOne(o => o.Tax, money =>
        {
            money.Property(m => m.Amount).HasColumnName("Tax").HasPrecision(18, 2);
            money.Property(m => m.Currency).HasColumnName("TaxCurrency").HasMaxLength(3);
        });

        builder.OwnsOne(o => o.TotalAmount, money =>
        {
            money.Property(m => m.Amount).HasColumnName("TotalAmount").HasPrecision(18, 2);
            money.Property(m => m.Currency).HasColumnName("TotalAmountCurrency").HasMaxLength(3);
        });

        builder.ComplexProperty(o => o.ShippingAddress, address =>
        {
            address.Property(a => a.Province).HasColumnName("ShippingProvince").HasMaxLength(100);
            address.Property(a => a.City).HasColumnName("ShippingCity").HasMaxLength(100);
            address.Property(a => a.PostalAddress).HasColumnName("ShippingPostalAddress").HasMaxLength(500);
            address.Property(a => a.PostalCode).HasColumnName("ShippingPostalCode").HasMaxLength(20);
            address.Property(a => a.Latitude).HasColumnName("ShippingLatitude").HasPrecision(9, 6);
            address.Property(a => a.Longitude).HasColumnName("ShippingLongitude").HasPrecision(9, 6);
            address.Property(a => a.Label).HasColumnName("ShippingLabel").HasMaxLength(100);
        });

        builder.ComplexProperty(o => o.BillingAddress, address =>
        {
            address.Property(a => a.Province).HasColumnName("BillingProvince").HasMaxLength(100);
            address.Property(a => a.City).HasColumnName("BillingCity").HasMaxLength(100);
            address.Property(a => a.PostalAddress).HasColumnName("BillingPostalAddress").HasMaxLength(500);
            address.Property(a => a.PostalCode).HasColumnName("BillingPostalCode").HasMaxLength(20);
            address.Property(a => a.Latitude).HasColumnName("BillingLatitude");
            address.Property(a => a.Longitude).HasColumnName("BillingLongitude");
            address.Property(a => a.Label).HasColumnName("BillingLabel").HasMaxLength(100);
        });

        builder.Property(o => o.ConfirmedAt);
        builder.Property(o => o.ProcessingAt);
        builder.Property(o => o.ShippedAt);
        builder.Property(o => o.DeliveredAt);
        builder.Property(o => o.CancelledAt);

        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
