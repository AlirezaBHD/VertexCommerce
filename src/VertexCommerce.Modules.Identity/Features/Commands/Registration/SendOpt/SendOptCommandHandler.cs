using Microsoft.Extensions.Caching.Memory;
using VertexCommerce.Modules.Identity.Domain.Repositories;
using VertexCommerce.Modules.Identity.Services;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Identity.Features.Commands.Registration.SendOpt;


internal sealed class SendOptCommandHandler(
    IMemoryCache cache,
    IOtpService  otpService,
    IUserRepository userRepository
    )
    : ICommandHandler<SendOptCommand, RegistrationTokenResponse>
{
    public async Task<Result<RegistrationTokenResponse>> Handle(SendOptCommand command, CancellationToken ct)
    {
        var phoneNumber = command.PhoneNumber;
        var phoneExists = await userRepository.PhoneExistsAsync(phoneNumber, ct);
        if (phoneExists)
        {
            return Result.Failure<RegistrationTokenResponse>(Error.Conflict("Phone number already registered"));
        }

        var otp = otpService.GenerateOtpAsync(phoneNumber, ct);
        await otpService.SendOtpAsync(phoneNumber, otp, ct);
        
        var registrationToken = Guid.NewGuid().ToString("N");

        var pending = new PendingRegistrationCache
        {
            PhoneNumber = phoneNumber,
            OtpCode = otp,
            OtpExpiresAt = DateTime.UtcNow.AddMinutes(5),
            IsPhoneVerified = false
        };

        cache.Set(
            $"reg:{registrationToken}",
            pending,
            TimeSpan.FromMinutes(30));

        return Result.Success(new RegistrationTokenResponse(
            RegistrationToken: registrationToken,
            ExpiresAt : DateTime.UtcNow.AddMinutes(30),
            NextStep : "verify_otp"
        ));
    }
}