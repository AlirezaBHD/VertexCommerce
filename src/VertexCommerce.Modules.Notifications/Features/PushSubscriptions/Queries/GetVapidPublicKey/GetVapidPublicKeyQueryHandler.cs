using Microsoft.Extensions.Options;
using VertexCommerce.Modules.Notifications.Services;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Notifications.Features.PushSubscriptions.Queries.GetVapidPublicKey;

internal sealed class GetVapidPublicKeyQueryHandler(IOptions<WebPushOptions> options)
    : IQueryHandler<GetVapidPublicKeyQuery, string>
{
    public Task<Result<string>> Handle(GetVapidPublicKeyQuery query, CancellationToken ct)
    {
        var settings = options.Value;
        var result = settings.IsEnabled
            ? Result.Success(settings.PublicKey)
            : Result.Failure<string>(WebPushErrors.Unavailable);
        return Task.FromResult(result);
    }
}
