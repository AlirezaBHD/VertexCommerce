using Microsoft.Extensions.Caching.Memory;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Identity.Features.Commands.Registration.VerifyOtp;


internal sealed class VerifyOtpCommandHandler(
    IMemoryCache cache
)
    : ICommandHandler<VerifyOtpCommand, RegistrationTokenResponse>
{
    public Task<Result<RegistrationTokenResponse>> Handle(VerifyOtpCommand command, CancellationToken ct)
    {
        var registrationToken = command.RegistrationToken;
        var otp = command.Otp;
        
        if (!cache.TryGetValue($"reg:{registrationToken}", out PendingRegistrationCache? pending) || pending is null)
        {
            return Task.FromResult(Result.Failure<RegistrationTokenResponse>(Error.Conflict("Token is expired.")));
        }

        if (DateTime.UtcNow > pending.OtpExpiresAt)
        {
            return Task.FromResult(Result.Failure<RegistrationTokenResponse>(Error.Conflict("Otp is expired.")));
        }

        if (pending.OtpCode != otp)
        {
            pending.RetryCount++;
            if (pending.RetryCount >= 5)
            {
                cache.Remove($"reg:{registrationToken}");
                return Task.FromResult(Result.Failure<RegistrationTokenResponse>(Error.Conflict("Too many attempts.")));
            }
            return Task.FromResult(Result.Failure<RegistrationTokenResponse>(Error.Conflict("Otp is wrong.")));
        }

        pending.IsPhoneVerified = true;
        cache.Set($"reg:{registrationToken}", pending, TimeSpan.FromMinutes(30));
        
        return Task.FromResult(Result.Success(new RegistrationTokenResponse(
            RegistrationToken: registrationToken,
            ExpiresAt: DateTime.UtcNow.AddMinutes(30),
            NextStep: "complete_profile"
        )));
    }
}
