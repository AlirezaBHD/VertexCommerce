using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Notifications.Features.PushSubscriptions.Commands.UnsubscribeFromPush;

public sealed record UnsubscribeFromPushCommand(string Endpoint) : ICommand;
