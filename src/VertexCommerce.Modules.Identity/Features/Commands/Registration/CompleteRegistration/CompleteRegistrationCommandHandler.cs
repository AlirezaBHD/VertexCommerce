using Microsoft.Extensions.Caching.Memory;
using VertexCommerce.Modules.Identity.Domain.Entities;
using VertexCommerce.Modules.Identity.Domain.Repositories;
using VertexCommerce.Modules.Identity.Persistence;
using VertexCommerce.Modules.Identity.Services;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Identity.Features.Commands.Registration.CompleteRegistration;

internal sealed class CompleteRegistrationCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IMemoryCache cache,
    IJwtService jwtService,
    IIdentityUnitOfWork unitOfWork)
    : ICommandHandler<CompleteRegistrationCommand, AuthResponse>
{
    public async Task<Result<AuthResponse>> Handle(CompleteRegistrationCommand command, CancellationToken ct)
    {
        if (!cache.TryGetValue($"reg:{command.RegistrationToken}", out PendingRegistrationCache? pending) ||
            pending is null)
        {
            return Result.Failure<AuthResponse>(Error.Conflict("Token is expired."));
        }

        if (!pending.IsPhoneVerified)
        {
            return Result.Failure<AuthResponse>(Error.Conflict("You need to verify OPT first."));
        }

        var passwordHash = passwordHasher.Hash(command.Password);

        var user = User.Create(
            phoneNumber: pending.PhoneNumber,
            passwordHash: passwordHash,
            firstName: command.FirstName,
            lastName: command.LastName
        );
        cache.Remove($"reg:{command.RegistrationToken}");

        var accessToken = jwtService.GenerateAccessToken(user);
        var refreshToken = jwtService.GenerateRefreshToken();
        var refreshTokenExpiry = jwtService.GetRefreshTokenExpiry();

        user.AddRefreshToken(refreshToken, refreshTokenExpiry);
        user.RecordLogin();

        await userRepository.AddAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new AuthResponse(
            UserId: user.Id,
            PhoneNumber: user.PhoneNumber,
            FullName: user.FullName,
            Role: user.Role.ToString(),
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            ExpiresAt: refreshTokenExpiry
        ));
    }
}
