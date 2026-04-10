using MoneyManager.Domain.Entities;

namespace MoneyManager.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<User?> FindByAccessKeyHashAsync(string accessKeyHash, CancellationToken ct = default);
    /// <summary>
    /// Ищет пользователя, у которого BCrypt.Verify(plainKey, storedHash) == true.
    /// Допустимо при малом числе пользователей (личное приложение).
    /// </summary>
    Task<User?> FindByAccessKeyPlainAsync(string plainKey, CancellationToken ct = default);
    Task<bool> AnyAsync(CancellationToken ct = default);
    Task AddAsync(User user, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
