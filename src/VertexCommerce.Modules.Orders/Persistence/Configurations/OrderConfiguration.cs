using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VertexCommerce.Modules.Orders.Domain.Entities;
using VertexCommerce.Modules.Orders.Domain.ValueObjects;
using VertexCommerce.Shared.Persistence;

namespace VertexCommerce.Modules.Orders.Persistence.Configurations;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        builder.HasKey(o => o.Id);

        builder.ComplexProperty(o => o.OrderNumber, orderNumber => {
            orderNumber.Property(p => p.Value).HasColumnName("OrderNumber").HasSchema(OrderNumber.Schema);
        });

        // builder.HasIndex("OrderNumber.Value").IsUnique(); // Manual migration needed for complex types
        builder.HasIndex(o => o.CustomerId);

        builder.ComplexProperty(o => o.CustomerPhoneNumber, phoneNumber => {
            phoneNumber.Property(p => p.Value).HasColumnName("CustomerPhoneNumber").HasSchema(PhoneNumber.Schema);
        });

        builder.Property(o => o.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(o => o.PaymentStatus)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(o => o.Notes).HasMaxLength(1000);
        builder.Property(o => o.CancellationReason).HasMaxLength(500);

        builder.Property(o => o.TrackingNumber)
            .HasConversion(
                t => t.HasValue ? t.Value.Value : null,
                v => string.IsNullOrEmpty(v) ? null : TrackingNumber.Create(v))
            .HasColumnName("TrackingNumber")
            .HasSchema(TrackingNumber.Schema);

        builder.ComplexProperty(o => o.SubTotal, money =>
        {
            money.Property(m => m.Amount).HasColumnName("SubTotal").HasPrecision(18, 2);
            money.Property(m => m.Currency).HasColumnName("SubTotalCurrency").HasSchema(Money.CurrencySchema);
        });

        builder.ComplexProperty(o => o.ShippingCost, money =>
        {
            money.Property(m => m.Amount).HasColumnName("ShippingCost").HasPrecision(18, 2);
            money.Property(m => m.Currency).HasColumnName("ShippingCostCurrency").HasSchema(Money.CurrencySchema);
        });

        builder.ComplexProperty(o => o.Tax, money =>
        {
            money.Property(m => m.Amount).HasColumnName("Tax").HasPrecision(18, 2);
            money.Property(m => m.Currency).HasColumnName("TaxCurrency").HasSchema(Money.CurrencySchema);
        });

        builder.ComplexProperty(o => o.TotalAmount, money =>
        {
            money.Property(m => m.Amount).HasColumnName("TotalAmount").HasPrecision(18, 2);
            money.Property(m => m.Currency).HasColumnName("TotalAmountCurrency").HasSchema(Money.CurrencySchema);
        });

        builder.ComplexProperty(o => o.ShippingAddress, address =>
        {
            address.ComplexProperty(a => a.Province, p => { p.Property(x => x.Value).HasColumnName("ShippingProvince").HasSchema(Province.Schema); });
            address.ComplexProperty(a => a.City, c => { c.Property(x => x.Value).HasColumnName("ShippingCity").HasSchema(City.Schema); });
            address.ComplexProperty(a => a.PostalAddress, pa => { pa.Property(x => x.Value).HasColumnName("ShippingPostalAddress").HasSchema(PostalAddress.Schema); });
            address.ComplexProperty(a => a.PostalCode, pc => { pc.Property(x => x.Value).HasColumnName("ShippingPostalCode").HasSchema(PostalCode.Schema); });
            address.ComplexProperty(a => a.Location, l =>
            {
                l.Property(x => x.Latitude).HasColumnName("ShippingLatitude").HasPrecision(9, 6);
                l.Property(x => x.Longitude).HasColumnName("ShippingLongitude").HasPrecision(9, 6);
            });
            // Label is nullable
            address.Property(a => a.Label)
                .HasConversion(
                    label => label.HasValue ? label.Value.Value : null,
                    value => string.IsNullOrEmpty(value) ? null : AddressLabel.CreateOrNull(value))
                .HasColumnName("ShippingLabel")
                .HasSchema(AddressLabel.Schema);
        });

        builder.ComplexProperty(o => o.BillingAddress, address =>
        {
            address.ComplexProperty(a => a.Province, p => { p.Property(x => x.Value).HasColumnName("BillingProvince").HasSchema(Province.Schema); });
            address.ComplexProperty(a => a.City, c => { c.Property(x => x.Value).HasColumnName("BillingCity").HasSchema(City.Schema); });
            address.ComplexProperty(a => a.PostalAddress, pa => { pa.Property(x => x.Value).HasColumnName("BillingPostalAddress").HasSchema(PostalAddress.Schema); });
            address.ComplexProperty(a => a.PostalCode, pc => { pc.Property(x => x.Value).HasColumnName("BillingPostalCode").HasSchema(PostalCode.Schema); });
            address.ComplexProperty(a => a.Location, l =>
            {
                l.Property(x => x.Latitude).HasColumnName("BillingLatitude").HasPrecision(9, 6);
                l.Property(x => x.Longitude).HasColumnName("BillingLongitude").HasPrecision(9, 6);
            });
            // Label is nullable
            address.Property(a => a.Label)
                .HasConversion(
                    label => label.HasValue ? label.Value.Value : null,
                    value => string.IsNullOrEmpty(value) ? null : AddressLabel.CreateOrNull(value))
                .HasColumnName("BillingLabel")
                .HasSchema(AddressLabel.Schema);
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
