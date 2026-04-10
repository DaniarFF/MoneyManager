using Microsoft.EntityFrameworkCore;
using MoneyManager.Domain.Entities;
using MoneyManager.Domain.Interfaces;
using MoneyManager.Infrastructure.Persistence;

namespace MoneyManager.Infrastructure.Repositories;

public class UserRepository(AppDbContext db, IPasswordHasher passwordHasher) : IUserRepository
{
    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<User?> FindByAccessKeyHashAsync(string accessKeyHash, CancellationToken ct = default) =>
        db.Users.FirstOrDefaultAsync(u => u.AccessKey == accessKeyHash, ct);

    public async Task<User?> FindByAccessKeyPlainAsync(string plainKey, CancellationToken ct = default)
    {
        // Загружаем всех пользователей для BCrypt-сравнения.
        // Для личного приложения пользователей единицы — это приемлемо.
        var users = await db.Users.ToListAsync(ct);
        return users.FirstOrDefault(u => passwordHasher.Verify(plainKey, u.AccessKey));
    }

    public Task<bool> AnyAsync(CancellationToken ct = default) =>
        db.Users.AnyAsync(ct);

    public async Task AddAsync(User user, CancellationToken ct = default) =>
        await db.Users.AddAsync(user, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        db.SaveChangesAsync(ct);
}
