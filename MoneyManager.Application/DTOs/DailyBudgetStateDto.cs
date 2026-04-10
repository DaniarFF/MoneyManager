namespace MoneyManager.Application.DTOs;

/// <summary>
/// Полное состояние бюджета на текущий день.
/// Рассчитывается "на лету" без snapshot-таблицы:
/// оставшийся бюджет делится на количество оставшихся дней месяца.
/// </summary>
public record DailyBudgetStateDto(
    decimal MonthlyBudget,
    decimal TotalSpentThisMonth,
    decimal RemainingMonthly,
    decimal TodayLimit,
    decimal TodaySpent,
    decimal TodayRemaining,
    decimal TomorrowLimit,
    string TomorrowChangeReason,
    int DaysRemainingInMonth,
    DateOnly Today
);
