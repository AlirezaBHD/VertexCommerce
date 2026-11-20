using System.Collections.Concurrent;
using System.Reflection;
using MediatR;
using Newtonsoft.Json;
using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Shared.Persistence.Outbox;

/// <summary>
/// Provides secure, high-performance JSON serialization and type-safe deserialization
/// for transactional outbox domain events without relying on insecure runtime type metadata.
/// </summary>
public static class OutboxSerializer
{
    private static readonly JsonSerializerSettings SerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.None,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        NullValueHandling = NullValueHandling.Ignore
    };

    private static readonly ConcurrentDictionary<string, Type?> TypeCache = new(StringComparer.Ordinal);

    /// <summary>
    /// Serializes a domain event payload to JSON with secure settings.
    /// </summary>
    /// <typeparam name="TEvent">The type of the domain event.</typeparam>
    /// <param name="domainEvent">The domain event instance.</param>
    /// <returns>Serialized JSON string.</returns>
    public static string Serialize<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        return JsonConvert.SerializeObject(domainEvent, SerializerSettings);
    }

    /// <summary>
    /// Deserializes an outbox payload into an <see cref="INotification"/> instance based on the stored event type name.
    /// </summary>
    /// <param name="typeName">The type name, full name, or assembly-qualified name of the event.</param>
    /// <param name="content">The JSON content of the event payload.</param>
    /// <returns>The deserialized <see cref="INotification"/> instance, or null if deserialization produces null.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the type is unknown or not an authorized domain event.</exception>
    public static INotification? Deserialize(string typeName, string content)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(typeName);
        ArgumentException.ThrowIfNullOrWhiteSpace(content);

        var targetType = ResolveEventType(typeName);
        if (targetType is null)
        {
            throw new InvalidOperationException(
                $"Cannot deserialize outbox message: Type '{typeName}' could not be resolved or is not an authorized domain event.");
        }

        return JsonConvert.DeserializeObject(content, targetType, SerializerSettings) as INotification;
    }

    /// <summary>
    /// Resolves the event type from the given type name, validating that it implements <see cref="INotification"/>
    /// and is a concrete class.
    /// </summary>
    /// <param name="typeName">The type name to resolve.</param>
    /// <returns>The resolved <see cref="Type"/>, or null if not found/unauthorized.</returns>
    public static Type? ResolveEventType(string typeName)
    {
        return TypeCache.GetOrAdd(typeName, static name =>
        {
            // 1. Direct type resolution (e.g. assembly-qualified name or full name in current AppDomain)
            var directType = Type.GetType(name, throwOnError: false);
            if (directType is not null && IsValidEventType(directType))
            {
                return directType;
            }

            // 2. Scan loaded assemblies (handles simple type name or moved namespaces)
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.IsDynamic)
                {
                    continue;
                }

                var matchedType = SafeGetTypes(assembly).FirstOrDefault(t =>
                    t.Name.Equals(name, StringComparison.Ordinal) ||
                    t.FullName?.Equals(name, StringComparison.Ordinal) == true);

                if (matchedType is not null && IsValidEventType(matchedType))
                {
                    return matchedType;
                }
            }

            return null;
        });
    }

    private static bool IsValidEventType(Type type) =>
        typeof(INotification).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface;

    private static IEnumerable<Type> SafeGetTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetExportedTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(static t => t is not null)!;
        }
        catch (Exception ex) when (ex is NotSupportedException or FileNotFoundException or FileLoadException)
        {
            return [];
        }
    }
}
