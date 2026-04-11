using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace MoneyManager.Services;

/// <summary>
/// Scoped-сервис. Вызови InitializeAsync() в OnInitializedAsync() страницы.
/// </summary>
public class UserSessionService(AuthenticationStateProvider authStateProvider)
{
    public Guid UserId { get; private set; }
    public string DisplayName { get; private set; } = "";
    public bool IsAuthenticated { get; private set; }

    private bool _initialized;

    public async Task InitializeAsync()
    {
        if (_initialized) return;
        _initialized = true;

        var state = await authStateProvider.GetAuthenticationStateAsync();
        var user = state.User;

        if (user.Identity?.IsAuthenticated == true)
        {
            var idStr = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            UserId = idStr != null ? Guid.Parse(idStr) : Guid.Empty;
            DisplayName = user.FindFirst(ClaimTypes.Name)?.Value ?? "Я";
            IsAuthenticated = true;
        }
    }
}
