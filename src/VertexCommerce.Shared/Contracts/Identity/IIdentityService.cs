namespace VertexCommerce.Shared.Contracts.Identity;

public interface IIdentityService
{
    Task<IReadOnlyList<Guid>> GetAdminUserIdsAsync(CancellationToken ct = default);
}
