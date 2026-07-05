using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VertexCommerce.Modules.Orders.Domain.Entities;

namespace VertexCommerce.Modules.Orders.Persistence.Configurations;

internal sealed class PaymentSettingsConfiguration : IEntityTypeConfiguration<PaymentSettings>
{
    public void Configure(EntityTypeBuilder<PaymentSettings> builder)
    {
        builder.ToTable("PaymentSettings");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.BankName).HasMaxLength(100).IsRequired();
        builder.Property(p => p.AccountHolderName).HasMaxLength(200).IsRequired();
        builder.Property(p => p.CardNumber).HasMaxLength(20).IsRequired();
        builder.Property(p => p.ShabaNumber).HasMaxLength(30);
        builder.Property(p => p.AccountNumber).HasMaxLength(50);
        builder.Property(p => p.Description).HasMaxLength(500);
        builder.Property(p => p.IsActive).IsRequired();

        builder.HasIndex(p => p.IsActive);
    }
}
