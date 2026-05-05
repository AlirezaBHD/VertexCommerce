using VertexCommerce.Modules.Identity.Domain.Repositories;
using VertexCommerce.Modules.Identity.Features.Commands.Registration.CompleteRegistration;
using VertexCommerce.Modules.Identity.Persistence;
using VertexCommerce.Modules.Identity.Services;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Identity.Features.Commands.RefreshToken;

internal sealed class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IIdentityUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        IJwtService jwtService,
        IIdentityUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand command, CancellationToken ct)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(command.RefreshToken, ct);

        if (user is null)
            return Result.Failure<AuthResponse>(Error.Unauthorized("Invalid refresh token"));

        var existingToken = user.RefreshTokens.FirstOrDefault(rt => rt.Token == command.RefreshToken);

        if (existingToken is null || !existingToken.IsActive)
            return Result.Failure<AuthResponse>(Error.Unauthorized("Invalid or expired refresh token"));

        // Revoke old token
        user.RevokeRefreshToken(command.RefreshToken);

        // Generate new tokens
        var accessToken = _jwtService.GenerateAccessToken(user);
        var newRefreshToken = _jwtService.GenerateRefreshToken();
        var refreshTokenExpiry = _jwtService.GetRefreshTokenExpiry();

        user.AddRefreshToken(newRefreshToken, refreshTokenExpiry);

        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new AuthResponse(
            user.Id,
            user.PhoneNumber,
            user.FullName,
            user.Role.ToString(),
            accessToken,
            newRefreshToken,
            refreshTokenExpiry
        ));
    }
}