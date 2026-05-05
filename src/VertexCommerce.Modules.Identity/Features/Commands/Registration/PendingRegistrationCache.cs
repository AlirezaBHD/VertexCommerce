namespace VertexCommerce.Modules.Identity.Features.Commands.Registration;

public class PendingRegistrationCache
{
    public string PhoneNumber { get; set; } = default!;
    public string OtpCode { get; set; } = default!;
    public DateTime OtpExpiresAt { get; set; }
    public bool IsPhoneVerified { get; set; }
    public int RetryCount { get; set; }
}