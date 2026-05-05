using Microsoft.Extensions.Logging;

namespace VertexCommerce.Modules.Identity.Services;

internal sealed class OtpService(ILogger<OtpService> logger) : IOtpService
{
    private const int _otpLength = 6;

    public string GenerateOtpAsync(string phoneNumber, CancellationToken ct)
    {
        var otp = GenerateRandomOtp();
        
        logger.LogInformation("OTP generated for phone number: {PhoneNumber}", phoneNumber);
        
        return otp;
    }

    public async Task SendOtpAsync(string phoneNumber, string otp, CancellationToken ct)
    {
        logger.LogInformation("Sending OTP {Otp} to phone number: {PhoneNumber}", otp, phoneNumber);
        //TODO
        await Task.CompletedTask;
    }

    private string GenerateRandomOtp()
    {
        var random = Random.Shared;
        var otp = random.Next(0, (int)Math.Pow(10, _otpLength)).ToString($"D{_otpLength}");
        return otp;
    }
}
