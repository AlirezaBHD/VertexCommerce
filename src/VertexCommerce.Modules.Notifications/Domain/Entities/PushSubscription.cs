using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Notifications.Domain.Entities;

public sealed class PushSubscription : Entity<Guid>
{
    public Guid UserId { get; private set; }
    public string Endpoint { get; private set; } = default!;
    public string P256dhKey { get; private set; } = default!;
    public string AuthKey { get; private set; } = default!;
    public string? UserAgent { get; private set; }

    private PushSubscription() { }

    public static PushSubscription Create(
        Guid userId,
        string endpoint,
        string p256DhKey,
        string authKey,
        string? userAgent = null)
    {
        return new PushSubscription
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Endpoint = endpoint.Trim(),
            P256dhKey = p256DhKey.Trim(),
            AuthKey = authKey.Trim(),
            UserAgent = userAgent?.Trim(),
            CreatedAt = DateTime.UtcNow
        };
    }
}
