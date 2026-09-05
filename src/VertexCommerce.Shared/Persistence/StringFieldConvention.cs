using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Shared.Persistence;

/// <summary>
/// EF Core model finalizing convention that automatically configures string properties
/// based on their [StringField] attribute annotations.
/// </summary>
public sealed class StringFieldConvention : IModelFinalizingConvention
{
    public void ProcessModelFinalizing(
        IConventionModelBuilder builder,
        IConventionContext<IConventionModelBuilder> context)
    {
        foreach (var entityType in builder.Metadata.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType != typeof(string)) continue;
                if (property.PropertyInfo?.GetCustomAttribute<StringFieldAttribute>() is not { } attr) continue;

                var spec = attr.ToSpec();
                
                // Note: MaxLength is handled natively because StringFieldAttribute derives from MaxLengthAttribute.
                // Note: IsRequired (Nullability) is deliberately omitted here. NRTs (string vs string?) are the single source of truth for nullability.
                
                property.Builder.IsUnicode(spec.IsUnicode, fromDataAnnotation: true);
                property.Builder.IsFixedLength(spec.IsFixedLength, fromDataAnnotation: true);
            }
        }
    }
}
