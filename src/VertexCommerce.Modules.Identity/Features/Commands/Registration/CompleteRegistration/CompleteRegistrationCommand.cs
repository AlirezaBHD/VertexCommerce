using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Identity.Features.Commands.Registration.CompleteRegistration;

public sealed record CompleteRegistrationCommand(
    string RegistrationToken,
    string Password,
    string FirstName,
    string LastName
) : ICommand<AuthResponse>;

public sealed record AuthResponse(
    Guid UserId,
    string PhoneNumber,
    string FullName,
    string Role,
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt
);
