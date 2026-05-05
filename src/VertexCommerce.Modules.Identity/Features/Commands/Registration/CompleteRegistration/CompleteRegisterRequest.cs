namespace VertexCommerce.Modules.Identity.Features.Commands.Registration.CompleteRegistration;

public sealed record CompleteRegisterRequest(
    string RegistrationToken,
    string Password,
    string FirstName,
    string LastName
);