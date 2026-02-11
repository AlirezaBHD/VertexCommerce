using VertexCommerce.Modules.Identity.Features.Register;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Identity.Features.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<AuthResponse>;
