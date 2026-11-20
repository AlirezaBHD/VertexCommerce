namespace VertexCommerce.Modules.Notifications.Features.Notifications.DTOs;

public sealed record NotificationDto(
    Guid Id,
    Guid UserId,
    string Title,
    string Message,
    string Type,
    string? PayloadJson,
    bool IsRead,
    DateTime? ReadAt,
    DateTime CreatedAt);
