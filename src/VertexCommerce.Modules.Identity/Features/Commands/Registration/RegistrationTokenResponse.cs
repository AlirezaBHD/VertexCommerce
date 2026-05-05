namespace VertexCommerce.Modules.Identity.Features.Commands.Registration;

public sealed record RegistrationTokenResponse(
    string RegistrationToken,
    DateTime ExpiresAt,
    string NextStep);
