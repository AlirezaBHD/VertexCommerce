namespace VertexCommerce.Modules.Identity.Features.Commands;

public sealed record AuthResponse(
    Guid UserId,
    string PhoneNumber,
    string FullName,
    string Role,
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt
);