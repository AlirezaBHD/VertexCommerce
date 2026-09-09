using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Modules.Customers.Domain.ValueObjects;

/// <summary>
/// A customer's family name. A distinct type from <see cref="FirstName"/> so the two cannot be
/// transposed at a call site.
/// </summary>
public readonly record struct LastName
{
    public static StringFieldSchema Schema { get; } = new(maxLength: 100);

    public string Value { get; }

    private LastName(string value)
    {
        Value = value;
    }

    public static LastName Create(string? value) =>
        new(StringFieldGuard.Apply(value, Schema, nameof(LastName)));
}
