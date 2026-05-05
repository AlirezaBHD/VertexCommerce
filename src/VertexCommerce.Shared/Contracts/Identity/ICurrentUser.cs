namespace VertexCommerce.Shared.Contracts.Identity;

public interface ICurrentUser
{
    Guid UserId { get; }
}