using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Identity.Features.Register;

public sealed record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName
) : ICommand<AuthResponse>;

public sealed record AuthResponse(
    Guid UserId,
    string Email,
    string FullName,
    string Role,
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt
);
