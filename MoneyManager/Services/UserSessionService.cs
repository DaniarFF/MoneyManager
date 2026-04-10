namespace MoneyManager.Services;

/// <summary>
/// Без авторизации — всегда возвращает фиксированного пользователя.
/// </summary>
public class UserSessionService
{
    // Фиксированный Guid дефолтного пользователя
    public static readonly Guid DefaultUserId = new("00000000-0000-0000-0000-000000000001");

    public Guid UserId => DefaultUserId;
    public string DisplayName => "Я";
    public bool IsAuthenticated => true;
}
