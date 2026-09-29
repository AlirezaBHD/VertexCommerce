namespace VertexCommerce.Modules.Notifications.Services.Models;

public sealed record NotificationMessage(
    Guid Id,
    Guid UserId,
    string Title,
    string Message,
    string Type,
    string? PayloadJson,
    bool IsRead,
    DateTime CreatedAt
);
