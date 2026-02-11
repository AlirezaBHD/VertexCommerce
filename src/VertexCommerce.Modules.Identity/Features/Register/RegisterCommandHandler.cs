using VertexCommerce.Modules.Identity.Domain.Entities;
using VertexCommerce.Modules.Identity.Domain.Repositories;
using VertexCommerce.Modules.Identity.Persistence;
using VertexCommerce.Modules.Identity.Services;
using VertexCommerce.Shared.CQRS;
using VertexCommerce.Shared.Persistence;

namespace VertexCommerce.Modules.Identity.Features.Register;

internal sealed class RegisterCommandHandler : ICommandHandler<RegisterCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly IIdentityUnitOfWork _unitOfWork;

    public RegisterCommandHandler(
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

    public async Task<Result<AuthResponse>> Handle(RegisterCommand command, CancellationToken ct)
    {
        var emailExists = await _userRepository.EmailExistsAsync(command.Email, ct);
        if (emailExists)
            return Result.Failure<AuthResponse>(Error.Conflict("Email already registered"));

        var passwordHash = _passwordHasher.Hash(command.Password);

        var user = User.Create(
            command.Email,
            passwordHash,
            command.FirstName,
            command.LastName
        );

        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();
        var refreshTokenExpiry = _jwtService.GetRefreshTokenExpiry();

        user.AddRefreshToken(refreshToken, refreshTokenExpiry);
        user.RecordLogin();

        await _userRepository.AddAsync(user, ct);
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