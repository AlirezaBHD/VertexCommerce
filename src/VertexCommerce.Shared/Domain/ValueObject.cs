using System.Diagnostics.CodeAnalysis;

namespace VertexCommerce.Shared.Domain;

/// <summary>
/// Base type for value objects: immutable objects defined entirely by their components.
/// </summary>
/// <remarks>
/// Two value objects are equal when they are of the same concrete type and their
/// <see cref="GetEqualityComponents"/> sequences are equal.
/// </remarks>
[SuppressMessage("Major Code Smell", "S3875:\"operator==\" should not be overloaded on reference types",
    Justification = "Value semantics are the defining trait of a value object; callers compare them with == and !=.")]
public abstract class ValueObject
{
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public override bool Equals(object? obj) =>
        obj is ValueObject other
        && other.GetType() == GetType()
        && GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Aggregate(0, (hash, component) =>
                HashCode.Combine(hash, component?.GetHashCode() ?? 0));
    }

    public static bool operator ==(ValueObject? left, ValueObject? right) => Equals(left, right);
    public static bool operator !=(ValueObject? left, ValueObject? right) => !Equals(left, right);
}
