using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VertexCommerce.Modules.Orders.Domain.Entities;
using VertexCommerce.Modules.Orders.Domain.ValueObjects;
using VertexCommerce.Shared.Persistence;

namespace VertexCommerce.Modules.Orders.Persistence.Configurations;

internal sealed class ShippingSettingsConfiguration : IEntityTypeConfiguration<ShippingSettings>
{
    public void Configure(EntityTypeBuilder<ShippingSettings> builder)
    {
        builder.ToTable("ShippingSettings");
        builder.HasKey(s => s.Id);

        builder.ComplexProperty(s => s.Cost, money =>
        {
            money.Property(m => m.Amount).HasColumnName("Cost").HasPrecision(18, 2);
            money.Property(m => m.Currency).HasColumnName("CostCurrency").HasSchema(Money.CurrencySchema);
        });

        builder.ComplexProperty(s => s.FreeShippingThreshold, money =>
        {
            money.Property(m => m.Amount).HasColumnName("FreeShippingThreshold").HasPrecision(18, 2);
            money.Property(m => m.Currency).HasColumnName("FreeShippingThresholdCurrency").HasSchema(Money.CurrencySchema);
        });

        builder.Property(s => s.Description)
            .HasMaxLength(500);

        builder.Property(s => s.IsActive)
            .IsRequired();

        builder.HasIndex(s => s.IsActive);
    }
}
