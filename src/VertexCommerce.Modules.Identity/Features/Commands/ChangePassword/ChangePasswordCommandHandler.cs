using VertexCommerce.Modules.Identity.Domain.Repositories;
using VertexCommerce.Modules.Identity.Persistence;
using VertexCommerce.Modules.Identity.Infrastructure.Authentication;
using VertexCommerce.Modules.Identity.Infrastructure.Cryptography;
using VertexCommerce.Modules.Identity.Infrastructure.Identity;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.CQRS;
using VertexCommerce.Modules.Identity.Domain.Errors;

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
            return Result.Failure(IdentityErrors.UserNotFound(userId));
        }

        var isValid = passwordHasher.Verify(command.CurrentPassword, user.PasswordHash);
        if (!isValid)
        {
            return Result.Failure(IdentityErrors.IncorrectCurrentPassword);
        }

        var newHash = passwordHasher.Hash(command.NewPassword);
        user.ChangePassword(newHash);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
