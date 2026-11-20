using VertexCommerce.Modules.Notifications.Services;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Notifications.Features.PushSubscriptions.Queries.GetVapidPublicKey;

internal sealed class GetVapidPublicKeyQueryHandler(IWebPushService webPushService)
    : IQueryHandler<GetVapidPublicKeyQuery, string>
{
    public Task<Result<string>> Handle(GetVapidPublicKeyQuery query, CancellationToken ct)
    {
        return Task.FromResult(Result.Success(webPushService.PublicKey));
    }
}
