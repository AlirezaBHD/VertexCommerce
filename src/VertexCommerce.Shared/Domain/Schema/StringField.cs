using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace VertexCommerce.Shared.Domain.Schema;

/// <summary>
/// Type-safe selector for resolving StringFieldSpec from entity properties decorated with [StringField].
/// </summary>
public static class StringField
{
    private static readonly ConcurrentDictionary<MemberInfo, StringFieldSpec> Cache = new();

    /// <summary>
    /// Resolves the StringFieldSpec declared on the property selected by the given expression.
    /// </summary>
    /// <typeparam name="TEntity">The entity type declaring the property.</typeparam>
    /// <param name="selector">Expression pointing to the entity property (e.g. c => c.FirstName).</param>
    /// <returns>The resolved StringFieldSpec.</returns>
    public static StringFieldSpec Of<TEntity>(Expression<Func<TEntity, string?>> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);

        var member = GetMemberInfo(selector);
        return Cache.GetOrAdd(member, static m =>
        {
            var attr = m.GetCustomAttribute<StringFieldAttribute>();
            if (attr is null)
            {
                throw new InvalidOperationException(
                    $"Property '{m.Name}' on type '{m.DeclaringType?.Name}' is not decorated with [{nameof(StringFieldAttribute)}].");
            }

            return attr.ToSpec();
        });
    }

    private static MemberInfo GetMemberInfo<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> expression)
    {
        var body = expression.Body;
        if (body is UnaryExpression { NodeType: ExpressionType.Convert } unary)
        {
            body = unary.Operand;
        }

        if (body is MemberExpression { Member: PropertyInfo prop })
        {
            return prop;
        }

        throw new ArgumentException($"Expression '{expression}' does not refer to a property on '{typeof(TEntity).Name}'.", nameof(expression));
    }
}
