using Microsoft.Extensions.Caching.Memory;
using VertexCommerce.Shared.CQRS;
using VertexCommerce.Modules.Identity.Domain.Errors;

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
            return Task.FromResult(Result.Failure<RegistrationTokenResponse>(IdentityErrors.TokenExpired));
        }

        if (DateTime.UtcNow > pending.OtpExpiresAt)
        {
            return Task.FromResult(Result.Failure<RegistrationTokenResponse>(IdentityErrors.OtpExpired));
        }

        if (pending.OtpCode != otp)
        {
            pending.RetryCount++;
            if (pending.RetryCount >= 5)
            {
                cache.Remove($"reg:{registrationToken}");
                return Task.FromResult(Result.Failure<RegistrationTokenResponse>(IdentityErrors.TooManyAttempts));
            }
            return Task.FromResult(Result.Failure<RegistrationTokenResponse>(IdentityErrors.OtpWrong));
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
