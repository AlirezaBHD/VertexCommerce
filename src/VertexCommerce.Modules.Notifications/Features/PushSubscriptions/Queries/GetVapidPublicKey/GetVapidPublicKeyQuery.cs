using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Notifications.Features.PushSubscriptions.Queries.GetVapidPublicKey;

public sealed record GetVapidPublicKeyQuery : IQuery<string>;
