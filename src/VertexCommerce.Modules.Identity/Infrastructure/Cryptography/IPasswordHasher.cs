namespace VertexCommerce.Modules.Identity.Infrastructure.Cryptography;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
