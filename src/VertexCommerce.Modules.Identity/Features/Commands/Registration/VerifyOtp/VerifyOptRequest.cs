namespace VertexCommerce.Modules.Identity.Features.Commands.Registration.VerifyOtp;

internal sealed record VerifyOptRequest(string RegistrationToken, string Opt);
