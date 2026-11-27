namespace VertexCommerce.Modules.Notifications.Endpoints;

public sealed record PushSubscribeRequest(
    string Endpoint,
    string P256dh,
    string Auth,
    string? UserAgent
);
