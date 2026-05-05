using VertexCommerce.Modules.Identity.Features.Commands.Registration.CompleteRegistration;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Identity.Features.Commands.Login;

public sealed record LoginCommand(
    string PhoneNumber,
    string Password
) : ICommand<AuthResponse>;
