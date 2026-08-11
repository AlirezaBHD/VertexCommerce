using VertexCommerce.Modules.Identity.Domain.Repositories;
using VertexCommerce.Modules.Identity.Features.Commands.Registration.CompleteRegistration;
using VertexCommerce.Modules.Identity.Persistence;
using VertexCommerce.Modules.Identity.Services;
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
            return Result.Failure<AuthResponse>(Error.Unauthorized("شماره موبایل یا رمز عبور اشتباه است."));

        if (!user.IsActive)
            return Result.Failure<AuthResponse>(Error.Unauthorized("حساب کاربری شما غیرفعال شده است."));

        if (!passwordHasher.Verify(command.Password, user.PasswordHash))
            return Result.Failure<AuthResponse>(Error.Unauthorized("شماره موبایل یا رمز عبور اشتباه است."));

        var accessToken = jwtService.GenerateAccessToken(user);
        var refreshToken = jwtService.GenerateRefreshToken();
        var refreshTokenExpiry = jwtService.GetRefreshTokenExpiry();

        user.AddRefreshToken(refreshToken, refreshTokenExpiry);
        user.RecordLogin();
        
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new AuthResponse(
            user.Id,
            user.PhoneNumber,
            user.FullName,
            user.Role.ToString(),
            accessToken,
            refreshToken,
            refreshTokenExpiry
        ));
    }
}