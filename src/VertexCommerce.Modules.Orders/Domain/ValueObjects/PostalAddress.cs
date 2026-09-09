using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Modules.Orders.Domain.ValueObjects;

/// <summary>The free-form street component of a postal address.</summary>
public readonly record struct PostalAddress
{
    public static StringFieldSchema Schema { get; } = new(maxLength: 500);

    public string Value { get; }

    private PostalAddress(string value)
    {
        Value = value;
    }

    public static PostalAddress Create(string? value) =>
        new(StringFieldGuard.Apply(value, Schema, nameof(PostalAddress)));
}

