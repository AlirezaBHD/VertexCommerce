using VertexCommerce.Modules.Identity.Domain.Enums;
using VertexCommerce.Modules.Identity.Domain.ValueObjects;
using VertexCommerce.Shared.Domain;
using VertexCommerce.Shared.IntegrationEvents;

namespace VertexCommerce.Modules.Identity.Domain.Entities;

public sealed class User : AggregateRoot<Guid>
{
    public PhoneNumber PhoneNumber { get; private set; }
    public string PasswordHash { get; private set; } = default!;
    public FirstName FirstName { get; private set; }
    public LastName LastName { get; private set; }
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    private readonly List<RefreshToken> _refreshTokens = [];
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens;

    private User() { }

    public static User Create(
        PhoneNumber phoneNumber,
        string passwordHash,
        FirstName firstName,
        LastName lastName,
        UserRole role = UserRole.User)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            PhoneNumber = phoneNumber,
            PasswordHash = passwordHash,
            FirstName = firstName,
            LastName = lastName,
            Role = role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        
        user.AddDomainEvent(new UserCreatedEvent(user.Id,  phoneNumber.Value, firstName.Value, lastName.Value));
        
        return user;
    }

    public string FullName => $"{FirstName.Value} {LastName.Value}";

    public void UpdateProfile(FirstName firstName, LastName lastName)
    {
        FirstName = firstName;
        LastName = lastName;
        SetUpdatedAt();
    }

    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        SetUpdatedAt();
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }

    public void Activate()
    {
        IsActive = true;
        SetUpdatedAt();
    }

    public RefreshToken AddRefreshToken(string token, DateTime expiresAt)
    {
        var refreshToken = RefreshToken.Create(Id, token, expiresAt);
        _refreshTokens.Add(refreshToken);
        return refreshToken;
    }

    public void RevokeRefreshToken(string token)
    {
        var refreshToken = _refreshTokens.FirstOrDefault(rt => rt.Token == token);
        refreshToken?.Revoke();
    }

    public void RevokeAllRefreshTokens()
    {
        foreach (var token in _refreshTokens.Where(rt => rt.IsActive))
        {
            token.Revoke();
        }
    }
}