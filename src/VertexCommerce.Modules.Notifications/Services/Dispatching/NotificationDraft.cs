namespace VertexCommerce.Modules.Notifications.Services.Dispatching;

internal sealed record NotificationDraft(
    string Title,
    string Message,
    string Type,
    string? PayloadJson,
    string? PushUrl,
    object? PushData);
