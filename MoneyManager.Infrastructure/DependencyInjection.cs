using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoneyManager.Domain.Interfaces;
using MoneyManager.Infrastructure.Persistence;
using MoneyManager.Infrastructure.Repositories;
using MoneyManager.Infrastructure.Security;

namespace MoneyManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = ResolveConnectionString(configuration);

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBudgetPlanRepository, BudgetPlanRepository>();
        services.AddScoped<IExpenseRepository, ExpenseRepository>();
        services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();

        return services;
    }

    public static async Task MigrateAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();
        try
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to apply database migrations. App will continue but may not work correctly.");
        }
    }

    /// <summary>
    /// Приоритет:
    /// 1. DATABASE_URL (Railway передаёт в формате postgresql://user:pass@host:port/db)
    /// 2. ConnectionStrings__DefaultConnection (env override)
    /// 3. appsettings.json ConnectionStrings:DefaultConnection
    /// </summary>
    private static string ResolveConnectionString(IConfiguration configuration)
    {
        var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
        if (!string.IsNullOrWhiteSpace(databaseUrl))
            return ConvertDatabaseUrl(databaseUrl);

        return configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "No database connection string found. Set DATABASE_URL or ConnectionStrings__DefaultConnection.");
    }

    /// <summary>
    /// Конвертирует postgresql://user:pass@host:port/db → Npgsql connection string.
    /// </summary>
    private static string ConvertDatabaseUrl(string databaseUrl)
    {
        // Railway даёт URL вида: postgresql://postgres:password@host:5432/railway
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':');
        var user = userInfo[0];
        var password = userInfo.Length > 1 ? userInfo[1] : "";
        var host = uri.Host;
        var port = uri.Port > 0 ? uri.Port : 5432;
        var database = uri.AbsolutePath.TrimStart('/');

        return $"Host={host};Port={port};Database={database};Username={user};Password={password};SSL Mode=Require;Trust Server Certificate=true";
    }
}
