using VertexCommerce.Modules.Identity.Domain.Repositories;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Identity.Features.Queries.GetProfile;

internal sealed class GetProfileQueryHandler(IUserRepository userRepository, ICurrentUser  currentUser)
    : IQueryHandler<GetProfileQuery, ProfileResponse>
{
    public async Task<Result<ProfileResponse>> Handle(GetProfileQuery query, CancellationToken ct)
    {
        var userId = currentUser.UserId;
        var user = await userRepository.GetByIdAsync(userId, ct);

        if (user is null)
        {
            return Result.Failure<ProfileResponse>(Error.NotFound("User", userId));
        }

        return Result.Success(new ProfileResponse(
            user.Id,
            user.PhoneNumber,
            user.FirstName,
            user.LastName,
            user.FullName,
            user.Role.ToString(),
            user.CreatedAt,
            user.LastLoginAt
        ));
    }
}
