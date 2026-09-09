using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Shared.Domain;

/// <summary>
/// Base class for a value object that wraps a single string, giving a distinct type to a
/// distinct domain concept so that two same-shaped concepts cannot be swapped by accident.
/// </summary>
/// <remarks>
/// Derived types declare a <c>static StringFieldSchema Schema</c> and expose a factory that routes
/// the raw value through <see cref="StringFieldGuard"/>. That single declaration is the source
/// of truth for the domain guard, the EF Core column mapping and the FluentValidation rules.
/// </remarks>
public abstract class StringValueObject : ValueObject
{
    public string Value { get; protected init; } = string.Empty;

    protected StringValueObject()
    {
    }

    protected sealed override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public sealed override string ToString() => Value;
}
