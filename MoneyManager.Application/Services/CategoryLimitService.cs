using MoneyManager.Application.Commands;
using MoneyManager.Application.DTOs;
using MoneyManager.Domain.Entities;
using MoneyManager.Domain.Interfaces;

namespace MoneyManager.Application.Services;

public class CategoryLimitService(
    ICategoryLimitRepository limitRepo,
    IExpenseRepository expenseRepo) : ICategoryLimitService
{
    public async Task<CategoryLimitStateDto> CreateAsync(CreateCategoryLimitCommand cmd, CancellationToken ct = default)
    {
        var limit = CategoryLimit.Create(cmd.UserId, cmd.Name, cmd.Icon, cmd.Color, cmd.LimitAmount);
        await limitRepo.AddAsync(limit, ct);
        await limitRepo.SaveChangesAsync(ct);
        return await BuildStateAsync(limit, ct);
    }

    public async Task<List<CategoryLimitStateDto>> GetActiveStatesAsync(Guid userId, CancellationToken ct = default)
    {
        var limits = await limitRepo.GetActiveByUserAsync(userId, ct);
        var result = new List<CategoryLimitStateDto>(limits.Count);
        foreach (var limit in limits)
            result.Add(await BuildStateAsync(limit, ct));
        return result;
    }

    public async Task ArchiveAsync(Guid limitId, CancellationToken ct = default)
    {
        var limit = await limitRepo.GetByIdAsync(limitId, ct)
            ?? throw new InvalidOperationException($"CategoryLimit {limitId} not found.");
        limit.Archive();
        await limitRepo.SaveChangesAsync(ct);
    }

    private async Task<CategoryLimitStateDto> BuildStateAsync(CategoryLimit limit, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var today = DateOnly.FromDateTime(now);

        // Расходы за весь период лимита
        var allExpenses = await expenseRepo.GetByUserCategoryAndPeriodAsync(
            limit.UserId, limit.Name, limit.PeriodStart, limit.PeriodEnd, ct);

        var totalSpent = allExpenses.Sum(e => e.Amount);
        var remaining = Math.Max(0, limit.LimitAmount - totalSpent);

        // Дни до конца периода включая сегодня
        var periodEnd = DateOnly.FromDateTime(limit.PeriodEnd);
        var daysRemaining = Math.Max(1, periodEnd.DayNumber - today.DayNumber + 1);

        // Расходы за сегодня
        var todayFrom = today.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var todayTo   = today.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);
        var todaySpent = allExpenses
            .Where(e => e.ExpenseDate >= todayFrom && e.ExpenseDate <= todayTo)
            .Sum(e => e.Amount);

        var todayLimit = daysRemaining > 0 ? remaining / daysRemaining : 0;
        var todayRemaining = todayLimit - todaySpent;

        // Прогноз на завтра
        var daysRemainingTomorrow = daysRemaining - 1;
        decimal tomorrowLimit = 0;
        string tomorrowReason;

        if (daysRemainingTomorrow > 0)
        {
            var remainingAfterToday = remaining - todaySpent;
            tomorrowLimit = Math.Max(0, remainingAfterToday / daysRemainingTomorrow);

            if (todaySpent < todayLimit)
                tomorrowReason = $"+{todayLimit - todaySpent:N0} ₽ сэкономлено сегодня";
            else if (todaySpent > todayLimit)
                tomorrowReason = $"−{todaySpent - todayLimit:N0} ₽ перерасход сегодня";
            else
                tomorrowReason = "Лимит сегодня соблюдён";
        }
        else
        {
            tomorrowReason = "Последний день периода";
        }

        // Статистика за последние 7 дней
        var recentDays = BuildRecentDays(allExpenses, limit, today, 7);

        return new CategoryLimitStateDto(
            LimitId: limit.Id,
            Name: limit.Name,
            Icon: limit.Icon,
            Color: limit.Color,
            LimitAmount: limit.LimitAmount,
            TotalSpent: totalSpent,
            Remaining: remaining,
            TodaySpent: todaySpent,
            TodayLimit: todayLimit,
            TodayRemaining: todayRemaining,
            TomorrowLimit: tomorrowLimit,
            TomorrowReason: tomorrowReason,
            DaysRemaining: daysRemaining,
            PeriodEndDate: periodEnd,
            RecentDays: recentDays
        );
    }

    private static List<DailyStatsDto> BuildRecentDays(
        List<Domain.Entities.Expense> expenses, CategoryLimit limit, DateOnly today, int days)
    {
        var result = new List<DailyStatsDto>(days);
        var periodStart = DateOnly.FromDateTime(limit.PeriodStart);
        var periodEnd   = DateOnly.FromDateTime(limit.PeriodEnd);

        var runningSpent = 0m;

        for (int i = days - 1; i >= 0; i--)
        {
            var date = today.AddDays(-i);
            if (date < periodStart || date > periodEnd)
            {
                result.Add(new DailyStatsDto(date, 0, 0, false));
                continue;
            }

            var dayFrom = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            var dayTo   = date.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);
            var daySpent = expenses
                .Where(e => e.ExpenseDate >= dayFrom && e.ExpenseDate <= dayTo)
                .Sum(e => e.Amount);

            var daysRemainingOnDate = periodEnd.DayNumber - date.DayNumber + 1;
            var remainingOnDate = Math.Max(0, limit.LimitAmount - runningSpent);
            var limitOnDate = daysRemainingOnDate > 0 ? remainingOnDate / daysRemainingOnDate : 0;

            result.Add(new DailyStatsDto(date, daySpent, limitOnDate, daySpent > limitOnDate));

            if (date < today)
                runningSpent += daySpent;
        }

        return result;
    }
}
