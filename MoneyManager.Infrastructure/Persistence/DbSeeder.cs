using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MoneyManager.Infrastructure.Persistence;

public static class DbSeeder
{
    private static readonly Guid LegacyDefaultUserId = new("00000000-0000-0000-0000-000000000001");

    /// <summary>
    /// Удаляет старого фиктивного пользователя, созданного до введения авторизации.
    /// </summary>
    public static async Task CleanupLegacyUserAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

        try
        {
            var deleted = await db.Database.ExecuteSqlRawAsync(
                "DELETE FROM users WHERE id = {0} AND access_key = {1}",
                LegacyDefaultUserId, "default-user-no-auth");

            if (deleted > 0)
                logger.LogInformation("Legacy no-auth user removed from database.");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not remove legacy user (may not exist).");
        }
    }
}
