using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Identity.Features.Commands.Registration.CompleteRegistration;

public sealed record CompleteRegistrationCommand(
    string RegistrationToken,
    string Password,
    string FirstName,
    string LastName
) : ICommand<AuthResponse>;
