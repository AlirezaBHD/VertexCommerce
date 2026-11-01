namespace VertexCommerce.Modules.Identity.Infrastructure.Authentication;

public interface IOtpService
{
    string GenerateOtp(string phoneNumber, CancellationToken ct);
    Task SendOtpAsync(string phoneNumber, string otp, CancellationToken ct);
}
