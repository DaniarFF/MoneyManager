using MoneyManager.Domain.Entities;
using MoneyManager.Domain.Interfaces;

namespace MoneyManager.Application.Services;

public class AuthService(
    IUserRepository userRepo,
    IPasswordHasher passwordHasher) : IAuthService
{
    public async Task<AuthResult> RegisterAsync(string displayName, string accessKey, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(accessKey) || accessKey.Length < 4)
            return new AuthResult(false, null, null, "Ключ доступа должен содержать минимум 4 символа.");

        var hash = passwordHasher.Hash(accessKey.Trim().ToLowerInvariant());

        // Проверяем, нет ли уже пользователя с таким ключом
        var existing = await userRepo.FindByAccessKeyHashAsync(hash, ct);
        if (existing is not null)
            return new AuthResult(false, null, null, "Такое слово уже занято. Придумайте другое.");

        var user = User.Create(displayName.Trim(), hash);
        await userRepo.AddAsync(user, ct);
        await userRepo.SaveChangesAsync(ct);

        return new AuthResult(true, user.Id, user.DisplayName, null);
    }

    public async Task<AuthResult> LoginAsync(string accessKey, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(accessKey))
            return new AuthResult(false, null, null, "Введите ключ доступа.");

        // Перебираем через hash-сравнение (BCrypt проверяет plaintext против hash)
        var normalizedKey = accessKey.Trim().ToLowerInvariant();

        // Получаем всех пользователей для bcrypt-сравнения
        // В личном бюджет-приложении пользователей единицы, поэтому это приемлемо
        var user = await userRepo.FindByAccessKeyPlainAsync(normalizedKey, ct);
        if (user is null)
            return new AuthResult(false, null, null, "Неверное слово-пароль.");

        return new AuthResult(true, user.Id, user.DisplayName, null);
    }

    public Task<bool> HasUsersAsync(CancellationToken ct = default) =>
        userRepo.AnyAsync(ct);
}
