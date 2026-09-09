using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Modules.Customers.Domain.ValueObjects;

/// <summary>
/// A customer's given name. A distinct type from <see cref="LastName"/> so the two cannot be
/// transposed at a call site.
/// </summary>
public readonly record struct FirstName
{
    public static StringFieldSchema Schema { get; } = new(maxLength: 100);

    public string Value { get; }

    private FirstName(string value)
    {
        Value = value;
    }

    public static FirstName Create(string? value) =>
        new(StringFieldGuard.Apply(value, Schema, nameof(FirstName)));
}
