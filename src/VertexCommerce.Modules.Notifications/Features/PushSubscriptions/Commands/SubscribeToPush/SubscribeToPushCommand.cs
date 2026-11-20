using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Notifications.Features.PushSubscriptions.Commands.SubscribeToPush;

public sealed record SubscribeToPushCommand(
    string Endpoint,
    string P256dh,
    string Auth,
    string? UserAgent = null) : ICommand;
