using MoneyManager.Application.Commands;
using MoneyManager.Application.DTOs;

namespace MoneyManager.Application.Services;

public interface IBudgetService
{
    /// <summary>
    /// Возвращает текущий план бюджета пользователя (текущий месяц).
    /// </summary>
    Task<BudgetPlanDto?> GetCurrentPlanAsync(Guid userId, CancellationToken ct = default);

    /// <summary>
    /// Создаёт или обновляет план бюджета на указанный месяц.
    /// </summary>
    Task<BudgetPlanDto> CreateOrUpdatePlanAsync(CreateBudgetPlanCommand command, CancellationToken ct = default);

    /// <summary>
    /// Полное состояние бюджета на сегодня: лимиты, потрачено, прогноз на завтра.
    /// </summary>
    Task<DailyBudgetStateDto?> GetDailyStateAsync(Guid userId, CancellationToken ct = default);

    /// <summary>
    /// Статистика по последним N дням (для мини-ленты на главном экране).
    /// </summary>
    Task<List<DailyStatsDto>> GetRecentDaysStatsAsync(Guid userId, int days = 7, CancellationToken ct = default);
}
