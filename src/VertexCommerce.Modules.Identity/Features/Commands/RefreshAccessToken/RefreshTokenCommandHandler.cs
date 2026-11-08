using VertexCommerce.Modules.Identity.Domain.Errors;
using VertexCommerce.Modules.Identity.Domain.Repositories;
using VertexCommerce.Modules.Identity.Infrastructure.Authentication;
using VertexCommerce.Modules.Identity.Persistence;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Identity.Features.Commands.RefreshAccessToken;

internal sealed class RefreshTokenCommandHandler(
    IUserRepository userRepository,
    IJwtService jwtService,
    IIdentityUnitOfWork unitOfWork)
    : ICommandHandler<RefreshTokenCommand, AuthResponse>
{
    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand command, CancellationToken ct)
    {
        var user = await userRepository.GetByRefreshTokenAsync(command.RefreshToken, ct);

        if (user is null)
            return Result.Failure<AuthResponse>(IdentityErrors.InvalidRefreshToken);

        var existingToken = user.RefreshTokens.FirstOrDefault(rt => rt.Token == command.RefreshToken);

        if (existingToken?.IsActive != true)
            return Result.Failure<AuthResponse>(IdentityErrors.InvalidRefreshToken);

        // Revoke old token
        user.RevokeRefreshToken(command.RefreshToken);

        // Generate new tokens
        var accessToken = jwtService.GenerateAccessToken(user);
        var newRefreshToken = jwtService.GenerateRefreshToken();
        var refreshTokenExpiry = jwtService.GetRefreshTokenExpiry();

        user.AddRefreshToken(newRefreshToken, refreshTokenExpiry);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new AuthResponse(
            user.Id,
            user.PhoneNumber.Value,
            user.FullName,
            user.Role.ToString(),
            accessToken,
            newRefreshToken,
            refreshTokenExpiry
        ));
    }
}
