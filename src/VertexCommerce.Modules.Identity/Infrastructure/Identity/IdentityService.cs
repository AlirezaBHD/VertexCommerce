using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VertexCommerce.Modules.Identity.Domain.Enums;
using VertexCommerce.Modules.Identity.Persistence;
using VertexCommerce.Shared.Contracts.Identity;

namespace VertexCommerce.Modules.Identity.Infrastructure.Identity;

internal sealed class IdentityService(IdentityDbContext dbContext, ILogger<IdentityService> logger) : IIdentityService
{
    public async Task<IReadOnlyList<Guid>> GetAdminUserIdsAsync(CancellationToken ct = default)
    {
        var admins = await dbContext.Users
            .AsNoTracking()
            .Where(u => u.Role == UserRole.Admin && u.IsActive)
            .Select(u => u.Id)
            .ToListAsync(ct);

        if (admins.Count == 0)
        {
            logger.LogWarning("No users found with Role == UserRole.Admin && IsActive! Inspecting all users in DB...");
            var allUsers = await dbContext.Users
                .AsNoTracking()
                .Select(u => new { u.Id, u.Role, u.IsActive, Phone = u.PhoneNumber.Value })
                .ToListAsync(ct);

            foreach (var u in allUsers)
            {
                logger.LogInformation("DB User: Id={Id}, Phone={Phone}, Role={Role}, IsActive={IsActive}",
                    u.Id, u.Phone, u.Role, u.IsActive);
            }

            admins = allUsers
                .Where(u => string.Equals(u.Role.ToString(), "Admin", StringComparison.OrdinalIgnoreCase))
                .Select(u => u.Id)
                .ToList();
        }

        return admins;
    }
}
