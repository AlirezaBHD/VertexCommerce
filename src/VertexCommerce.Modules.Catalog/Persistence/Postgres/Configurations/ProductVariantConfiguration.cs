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
            skuBuilder.Property(s => s.Value).HasColumnName("sku").IsRequired();
        });

        builder.ComplexProperty(v => v.Price, priceBuilder => 
        {
            priceBuilder.Property(p => p.Amount).HasColumnName("price_amount").IsRequired();
            priceBuilder.Property(p => p.Currency).HasColumnName("price_currency").IsRequired();
        });

        builder.OwnsMany(v => v.Attributes, ob =>
        {
            ob.ToJson();
            ob.Property(o => o.AttributeCode).HasJsonPropertyName("attribute_code").IsRequired();
            ob.Property(o => o.OptionCode).HasJsonPropertyName("option_code").IsRequired();
        });
    }
}
