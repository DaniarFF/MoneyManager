using Microsoft.Extensions.DependencyInjection;
using MoneyManager.Application.Services;

namespace MoneyManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IBudgetService, BudgetService>();
        services.AddScoped<IExpenseService, ExpenseService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICategoryLimitService, CategoryLimitService>();
        services.AddScoped<IDebtService, DebtService>();
        return services;
    }
}
