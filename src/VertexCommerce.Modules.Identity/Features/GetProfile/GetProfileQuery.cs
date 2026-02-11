using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Identity.Features.GetProfile;

public sealed record GetProfileQuery(Guid UserId) : IQuery<ProfileResponse>;

public sealed record ProfileResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string FullName,
    string Role,
    DateTime CreatedAt,
    DateTime? LastLoginAt
);
