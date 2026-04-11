using MoneyManager.Domain.Interfaces;

namespace MoneyManager.Infrastructure.Security;

public class BcryptPasswordHasher : IPasswordHasher
{
    public string Hash(string plainText) =>
        BCrypt.Net.BCrypt.HashPassword(plainText, workFactor: 11);

    public bool Verify(string plainText, string hash)
    {
        try { return BCrypt.Net.BCrypt.Verify(plainText, hash); }
        catch (BCrypt.Net.SaltParseException) { return false; }
    }
}
