namespace MoneyManager.Application.Services;

public record AuthResult(bool Success, Guid? UserId, string? DisplayName, string? Error);

public interface IAuthService
{
    /// <summary>
    /// Регистрация: создаёт пользователя с уникальным словом-паролем.
    /// </summary>
    Task<AuthResult> RegisterAsync(string displayName, string accessKey, CancellationToken ct = default);

    /// <summary>
    /// Вход: проверяет слово-пароль и возвращает идентификатор пользователя.
    /// </summary>
    Task<AuthResult> LoginAsync(string accessKey, CancellationToken ct = default);

    /// <summary>
    /// Проверяет, есть ли хотя бы один зарегистрированный пользователь.
    /// </summary>
    Task<bool> HasUsersAsync(CancellationToken ct = default);
}
