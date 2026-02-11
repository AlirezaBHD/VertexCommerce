using VertexCommerce.Modules.Identity.Features.Register;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Identity.Features.Login;

public sealed record LoginCommand(
    string Email,
    string Password
) : ICommand<AuthResponse>;
