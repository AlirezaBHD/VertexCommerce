namespace VertexCommerce.Modules.Identity.Services;

public interface IOtpService
{
    string GenerateOtpAsync(string phoneNumber, CancellationToken ct);
    Task SendOtpAsync(string phoneNumber, string otp, CancellationToken ct);
}