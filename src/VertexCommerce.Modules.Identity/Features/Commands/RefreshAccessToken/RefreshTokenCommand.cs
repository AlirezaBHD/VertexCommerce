using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Identity.Features.Commands.RefreshAccessToken;

public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<AuthResponse>;
