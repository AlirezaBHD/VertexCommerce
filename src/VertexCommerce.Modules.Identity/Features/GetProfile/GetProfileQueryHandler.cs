using VertexCommerce.Modules.Identity.Domain.Repositories;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Identity.Features.GetProfile;

internal sealed class GetProfileQueryHandler : IQueryHandler<GetProfileQuery, ProfileResponse>
{
    private readonly IUserRepository _userRepository;

    public GetProfileQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<ProfileResponse>> Handle(GetProfileQuery query, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(query.UserId, ct);

        if (user is null)
            return Result.Failure<ProfileResponse>(Error.NotFound("User", query.UserId));

        return Result.Success(new ProfileResponse(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.FullName,
            user.Role.ToString(),
            user.CreatedAt,
            user.LastLoginAt
        ));
    }
}
