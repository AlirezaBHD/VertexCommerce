using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VertexCommerce.Shared.Domain;
using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Shared.Persistence;

/// <summary>
/// Projects a <see cref="StringFieldSchema"/> onto an EF Core column, so the storage shape of a
/// value object is derived from the same declaration that guards it in the domain.
/// </summary>
/// <remarks>
/// Nullability is deliberately not configured here: C# nullable reference types remain the
/// single source of truth for whether a column is required.
/// </remarks>
public static class StringFieldSchemaMappingExtensions
{
    public static PropertyBuilder<string> HasSchema(
        this PropertyBuilder<string> property, StringFieldSchema schema)
    {
        ArgumentNullException.ThrowIfNull(schema);

        return property
            .HasMaxLength(schema.MaxLength)
            .IsUnicode(schema.IsUnicode)
            .IsFixedLength(schema.IsFixedLength);
    }

    public static ComplexTypePropertyBuilder<string> HasSchema(
        this ComplexTypePropertyBuilder<string> property, StringFieldSchema schema)
    {
        ArgumentNullException.ThrowIfNull(schema);

        return property
            .HasMaxLength(schema.MaxLength)
            .IsUnicode(schema.IsUnicode)
            .IsFixedLength(schema.IsFixedLength);
    }

    /// <summary>
    /// Applies a spec to a value object stored through a value converter rather than as a complex
    /// type, which is how an optional string-backed value object has to be mapped until EF Core
    /// supports optional complex types.
    /// </summary>
    public static ComplexTypePropertyBuilder<TValueObject> HasSchema<TValueObject>(this ComplexTypePropertyBuilder<TValueObject> property, StringFieldSchema schema) { ArgumentNullException.ThrowIfNull(schema); return property.HasMaxLength(schema.MaxLength).IsUnicode(schema.IsUnicode).IsFixedLength(schema.IsFixedLength); }

    public static PropertyBuilder<TValueObject> HasSchema<TValueObject>(
        this PropertyBuilder<TValueObject> property, StringFieldSchema schema)
    {
        ArgumentNullException.ThrowIfNull(schema);

        return property
            .HasMaxLength(schema.MaxLength)
            .IsUnicode(schema.IsUnicode)
            .IsFixedLength(schema.IsFixedLength);
    }
}
