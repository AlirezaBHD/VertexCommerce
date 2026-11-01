using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Identity.Domain.Entities;
using VertexCommerce.Modules.Identity.Domain.Repositories;
using VertexCommerce.Modules.Identity.Domain.ValueObjects;

namespace VertexCommerce.Modules.Identity.Persistence.Repositories;

internal sealed class UserRepository(IdentityDbContext context) : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<User?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken ct = default)
    {
        var normalized = PhoneNumber.Normalize(phoneNumber);
        return await context.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.PhoneNumber.Value == normalized || u.PhoneNumber.Value == phoneNumber, ct);
    }

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        return await context.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken), ct);
    }

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        await context.Users.AddAsync(user, ct);
    }

    public void Update(User user)
    {
        context.Users.Update(user);
    }

    public async Task<bool> PhoneExistsAsync(string phoneNumber, CancellationToken ct)
    {
        var normalized = PhoneNumber.Normalize(phoneNumber);
        return await context.Users.AnyAsync(u => u.PhoneNumber.Value == normalized || u.PhoneNumber.Value == phoneNumber, ct);
    }
}
