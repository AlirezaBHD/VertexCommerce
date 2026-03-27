using VertexCommerce.Modules.Identity.Domain.Repositories;
using VertexCommerce.Modules.Identity.Features.Register;
using VertexCommerce.Modules.Identity.Persistence;
using VertexCommerce.Modules.Identity.Services;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Identity.Features.Login;

internal sealed class LoginCommandHandler : ICommandHandler<LoginCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly IIdentityUnitOfWork _unitOfWork;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService,
        IIdentityUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand command, CancellationToken ct)
    {
        var user = await _userRepository.GetByEmailAsync(command.Email, ct);

        if (user is null)
            return Result.Failure<AuthResponse>(Error.Unauthorized("Invalid email or password"));

        if (!user.IsActive)
            return Result.Failure<AuthResponse>(Error.Unauthorized("Account is deactivated"));

        if (!_passwordHasher.Verify(command.Password, user.PasswordHash))
            return Result.Failure<AuthResponse>(Error.Unauthorized("Invalid email or password"));

        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();
        var refreshTokenExpiry = _jwtService.GetRefreshTokenExpiry();

        user.AddRefreshToken(refreshToken, refreshTokenExpiry);
        user.RecordLogin();
        
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new AuthResponse(
            user.Id,
            user.Email,
            user.FullName,
            user.Role.ToString(),
            accessToken,
            refreshToken,
            refreshTokenExpiry
        ));
    }
}