using VertexCommerce.Modules.Identity.Domain.Repositories;
using VertexCommerce.Modules.Identity.Persistence;
using VertexCommerce.Modules.Identity.Services;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Identity.Features.Commands.ChangePassword;

internal sealed class ChangePasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IIdentityUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : ICommandHandler<ChangePasswordCommand>
{
    public async Task<Result> Handle(ChangePasswordCommand command, CancellationToken ct)
    {
        var userId = currentUser.UserId;
        var user = await userRepository.GetByIdAsync(userId, ct);
        if (user is null)
        {
            return Result.Failure(Error.NotFound("User", userId));
        }

        var isValid = passwordHasher.Verify(command.CurrentPassword, user.PasswordHash);
        if (!isValid)
        {
            return Result.Failure(Error.Validation("Current password is incorrect"));
        }

        var newHash = passwordHasher.Hash(command.NewPassword);
        user.ChangePassword(newHash);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
