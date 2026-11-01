namespace VertexCommerce.Modules.Identity.Features.Queries.GetProfile;

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