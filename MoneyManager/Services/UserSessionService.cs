using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace MoneyManager.Services;

public class UserSessionService
{
    public Guid UserId { get; }
    public string DisplayName { get; } = "";
    public bool IsAuthenticated { get; }

    public UserSessionService(IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated == true)
        {
            var idStr = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            UserId = idStr != null ? Guid.Parse(idStr) : Guid.Empty;
            DisplayName = user.FindFirst(ClaimTypes.Name)?.Value ?? "Я";
            IsAuthenticated = true;
        }
    }
}
