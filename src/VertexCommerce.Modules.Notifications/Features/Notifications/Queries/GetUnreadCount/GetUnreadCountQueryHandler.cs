using VertexCommerce.Modules.Notifications.Domain.Repositories;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Notifications.Features.Notifications.Queries.GetUnreadCount;

internal sealed class GetUnreadCountQueryHandler(
    INotificationRepository repository,
    ICurrentUser currentUser)
    : IQueryHandler<GetUnreadCountQuery, int>
{
    public async Task<Result<int>> Handle(GetUnreadCountQuery query, CancellationToken ct)
    {
        var count = await repository.GetUnreadCountAsync(currentUser.GetUserId(), ct);
        return Result.Success(count);
    }
}
