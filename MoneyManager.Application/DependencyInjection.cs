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
        return services;
    }
}
