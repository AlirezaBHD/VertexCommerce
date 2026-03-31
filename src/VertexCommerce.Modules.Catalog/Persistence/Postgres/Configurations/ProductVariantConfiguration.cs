using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VertexCommerce.Modules.Catalog.Domain.Products;

namespace VertexCommerce.Modules.Catalog.Persistence.Postgres.Configurations;

public sealed class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("product_variants", "catalog");
        builder.HasKey(v => v.Id);

        
        builder.ComplexProperty(v => v.Sku, skuBuilder => 
        {
            skuBuilder.Property(s => s.Value).HasColumnName("Sku").IsRequired();
        });

        builder.ComplexProperty(v => v.Price, priceBuilder => 
        {
            priceBuilder.Property(p => p.Amount).HasColumnName("PriceAmount");
            priceBuilder.Property(p => p.Currency).HasColumnName("PriceCurrency");
        });

        builder.OwnsMany(v => v.Options, ob =>
        {
            ob.ToJson();
            ob.Property(o => o.Name).IsRequired();
            ob.Property(o => o.Value).IsRequired();
        });

        builder.OwnsMany(v => v.Media, mb =>
        {
            mb.ToJson();
            mb.Property(m => m.Path).IsRequired();
            mb.Property(m => m.Type).IsRequired();
        });
    }
}
