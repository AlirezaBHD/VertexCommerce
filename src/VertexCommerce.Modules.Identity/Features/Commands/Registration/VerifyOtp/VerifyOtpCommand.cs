using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Identity.Features.Commands.Registration.VerifyOtp;

public sealed record VerifyOtpCommand(string RegistrationToken, string Otp) : ICommand<RegistrationTokenResponse>;