using VertexCommerce.Modules.Identity.Features.Commands.Registration.CompleteRegistration;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Identity.Features.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<AuthResponse>;
