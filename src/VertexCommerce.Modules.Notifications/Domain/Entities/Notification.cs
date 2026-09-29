using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Notifications.Domain.Entities;

public sealed class Notification : Entity<Guid>
{
    public Guid UserId { get; private set; }
    public string Title { get; private set; } = default!;
    public string Message { get; private set; } = default!;
    public string Type { get; private set; } = default!;
    public string? PayloadJson { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }

    private Notification() { }

    public static Notification Create(
        Guid userId,
        string title,
        string message,
        string type,
        string? payloadJson = null)
    {
        return new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = title.Trim(),
            Message = message.Trim(),
            Type = type,
            PayloadJson = payloadJson,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkAsRead()
    {
        if (!IsRead)
        {
            IsRead = true;
            ReadAt = DateTime.UtcNow;
        }
    }
}
