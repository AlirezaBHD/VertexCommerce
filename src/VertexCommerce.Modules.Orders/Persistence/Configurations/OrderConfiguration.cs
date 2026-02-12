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

        builder.Property(o => o.CustomerEmail)
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

        builder.OwnsOne(o => o.ShippingAddress, address =>
        {
            address.Property(a => a.Street).HasColumnName("ShippingStreet").HasMaxLength(200);
            address.Property(a => a.City).HasColumnName("ShippingCity").HasMaxLength(100);
            address.Property(a => a.State).HasColumnName("ShippingState").HasMaxLength(100);
            address.Property(a => a.Country).HasColumnName("ShippingCountry").HasMaxLength(100);
            address.Property(a => a.ZipCode).HasColumnName("ShippingZipCode").HasMaxLength(20);
        });

        builder.OwnsOne(o => o.BillingAddress, address =>
        {
            address.Property(a => a.Street).HasColumnName("BillingStreet").HasMaxLength(200);
            address.Property(a => a.City).HasColumnName("BillingCity").HasMaxLength(100);
            address.Property(a => a.State).HasColumnName("BillingState").HasMaxLength(100);
            address.Property(a => a.Country).HasColumnName("BillingCountry").HasMaxLength(100);
            address.Property(a => a.ZipCode).HasColumnName("BillingZipCode").HasMaxLength(20);
        });

        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}