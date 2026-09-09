using VertexCommerce.Modules.Identity.Domain.Errors;
using VertexCommerce.Modules.Identity.Domain.Repositories;
using VertexCommerce.Modules.Identity.Features.Commands.Registration.CompleteRegistration;
using VertexCommerce.Modules.Identity.Persistence;
using VertexCommerce.Modules.Identity.Infrastructure.Authentication;
using VertexCommerce.Modules.Identity.Infrastructure.Cryptography;
using VertexCommerce.Modules.Identity.Infrastructure.Identity;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Identity.Features.Commands.Login;

internal sealed class LoginCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtService jwtService,
    IIdentityUnitOfWork unitOfWork)
    : ICommandHandler<LoginCommand, AuthResponse>
{
    public async Task<Result<AuthResponse>> Handle(LoginCommand command, CancellationToken ct)
    {
        var user = await userRepository.GetByPhoneNumberAsync(command.PhoneNumber, ct);

        if (user is null)
            return Result.Failure<AuthResponse>(IdentityErrors.InvalidCredentials);

        if (!user.IsActive)
            return Result.Failure<AuthResponse>(IdentityErrors.AccountInactive);

        if (!passwordHasher.Verify(command.Password, user.PasswordHash))
            return Result.Failure<AuthResponse>(IdentityErrors.InvalidCredentials);

        var accessToken = jwtService.GenerateAccessToken(user);
        var refreshToken = jwtService.GenerateRefreshToken();
        var refreshTokenExpiry = jwtService.GetRefreshTokenExpiry();

        user.AddRefreshToken(refreshToken, refreshTokenExpiry);
        user.RecordLogin();
        
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new AuthResponse(
            user.Id,
            user.PhoneNumber.Value,
            user.FullName,
            user.Role.ToString(),
            accessToken,
            refreshToken,
            refreshTokenExpiry
        ));
    }
}
