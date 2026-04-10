using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoneyManager.Domain.Entities;

namespace MoneyManager.Infrastructure.Persistence;

public static class DbSeeder
{
    public static readonly Guid DefaultUserId = new("00000000-0000-0000-0000-000000000001");

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

        try
        {
            var exists = await db.Users.AnyAsync(u => u.Id == DefaultUserId);
            if (!exists)
            {
                var user = User.Create("Я", "default-user-no-auth");
                // Вставляем напрямую через EF с фиксированным Id
                await db.Database.ExecuteSqlRawAsync(
                    "INSERT INTO users (id, display_name, access_key, created_at) VALUES ({0}, {1}, {2}, {3})",
                    DefaultUserId, "Я", "default-user-no-auth", DateTime.UtcNow);
                logger.LogInformation("Default user seeded.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to seed default user.");
        }
    }
}
