namespace MoneyManager.Services;

/// <summary>
/// Хранит состояние авторизованного пользователя в рамках Blazor-сессии.
/// Scoped-сервис: живёт пока живёт SignalR-соединение.
/// </summary>
public class UserSessionService
{
    public Guid? UserId { get; private set; }
    public string? DisplayName { get; private set; }
    public bool IsAuthenticated => UserId.HasValue;

    public event Action? OnChange;

    public void SetUser(Guid userId, string displayName)
    {
        UserId = userId;
        DisplayName = displayName;
        NotifyStateChanged();
    }

    public void Clear()
    {
        UserId = null;
        DisplayName = null;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
