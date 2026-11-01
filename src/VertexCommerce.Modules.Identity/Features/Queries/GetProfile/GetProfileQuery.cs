using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Identity.Features.Queries.GetProfile;

public sealed record GetProfileQuery : IQuery<ProfileResponse>;