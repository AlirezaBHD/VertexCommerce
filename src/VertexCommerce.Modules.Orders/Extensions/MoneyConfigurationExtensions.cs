using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VertexCommerce.Modules.Orders.Domain.ValueObjects;

namespace VertexCommerce.Modules.Orders.Extensions;

internal static class MoneyConfigurationExtensions
{
    internal static ComplexPropertyBuilder<Money> ConfigureMoney(
        this ComplexPropertyBuilder<Money> builder,
        string columnNamePrefix)
    {
        builder.Property(m => m.Amount)
            .HasColumnName(columnNamePrefix)
            .HasPrecision(18, 2);

        builder.Property(m => m.Currency)
            .HasColumnName($"{columnNamePrefix}Currency");
        return builder;
    }
}