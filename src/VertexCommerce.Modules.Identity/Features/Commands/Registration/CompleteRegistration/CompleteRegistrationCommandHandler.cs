using Microsoft.Extensions.Caching.Memory;
using VertexCommerce.Modules.Identity.Domain.Entities;
using VertexCommerce.Modules.Identity.Domain.ValueObjects;
using VertexCommerce.Modules.Identity.Domain.Repositories;
using VertexCommerce.Modules.Identity.Persistence;
using VertexCommerce.Modules.Identity.Infrastructure.Authentication;
using VertexCommerce.Modules.Identity.Infrastructure.Cryptography;
using VertexCommerce.Modules.Identity.Infrastructure.Identity;
using VertexCommerce.Shared.CQRS;
using VertexCommerce.Modules.Identity.Domain.Errors;

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
            return Result.Failure<AuthResponse>(IdentityErrors.TokenExpired);
        }

        if (!pending.IsPhoneVerified)
        {
            return Result.Failure<AuthResponse>(IdentityErrors.OptNotVerified);
        }

        var passwordHash = passwordHasher.Hash(command.Password);

        var user = User.Create(
            phoneNumber: PhoneNumber.Create(pending.PhoneNumber),
            passwordHash: passwordHash,
            firstName: FirstName.Create(command.FirstName),
            lastName: LastName.Create(command.LastName)
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
            PhoneNumber: user.PhoneNumber.Value,
            FullName: user.FullName,
            Role: user.Role.ToString(),
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            ExpiresAt: refreshTokenExpiry
        ));
    }
}
