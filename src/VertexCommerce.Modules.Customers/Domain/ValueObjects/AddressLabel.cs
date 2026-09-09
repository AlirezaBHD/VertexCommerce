using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Modules.Customers.Domain.ValueObjects;

/// <summary>
/// An optional user-supplied nickname for a saved address, such as "خانه" or "محل کار".
/// </summary>
public readonly record struct AddressLabel
{
    /// <remarks>
    /// <see cref="StringFieldSchema.AllowEmpty"/> is set because a blank label is legitimate input at
    /// the API boundary. The domain models that absence as <c>null</c> rather than as an empty
    /// instance, so a label that does exist is always meaningful.
    /// </remarks>
    public static StringFieldSchema Schema { get; } = new(maxLength: 50, allowEmpty: true);

    public string Value { get; }

    private AddressLabel(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Returns <c>null</c> when no label was supplied, otherwise a validated label.
    /// </summary>
    public static AddressLabel? CreateOrNull(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? null
            : new AddressLabel(StringFieldGuard.Apply(value, Schema, nameof(AddressLabel)));
}
