using MoneyManager.Application.Commands;
using MoneyManager.Application.DTOs;
using MoneyManager.Domain.Entities;
using MoneyManager.Domain.Interfaces;

namespace MoneyManager.Application.Services;

public class BudgetService(
    IBudgetPlanRepository budgetRepo,
    IExpenseRepository expenseRepo) : IBudgetService
{
    public async Task<BudgetPlanDto?> GetCurrentPlanAsync(Guid userId, CancellationToken ct = default)
    {
        var plan = await budgetRepo.GetCurrentAsync(userId, ct);
        return plan is null ? null : MapToDto(plan);
    }

    public async Task<BudgetPlanDto> CreateOrUpdatePlanAsync(CreateBudgetPlanCommand cmd, CancellationToken ct = default)
    {
        var existing = await budgetRepo.GetByUserAndMonthAsync(cmd.UserId, cmd.Month, cmd.Year, ct);

        if (existing is not null)
        {
            existing.UpdateAmount(cmd.MonthlyAmount);
            await budgetRepo.SaveChangesAsync(ct);
            return MapToDto(existing);
        }

        var plan = BudgetPlan.Create(cmd.UserId, cmd.Month, cmd.Year, cmd.MonthlyAmount);
        await budgetRepo.AddAsync(plan, ct);
        await budgetRepo.SaveChangesAsync(ct);
        return MapToDto(plan);
    }

    public async Task<DailyBudgetStateDto?> GetDailyStateAsync(Guid userId, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var today = DateOnly.FromDateTime(now);

        var plan = await budgetRepo.GetCurrentAsync(userId, ct);
        if (plan is null) return null;

        var totalSpent = await expenseRepo.GetTotalSpentAsync(plan.Id, ct);
        var todaySpent = await expenseRepo.GetSpentOnDateAsync(plan.Id, today, ct);

        var daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);
        // Оставшиеся дни включая сегодня
        var daysRemaining = daysInMonth - today.Day + 1;

        var remainingMonthly = plan.MonthlyAmount - totalSpent;

        // Лимит на сегодня = остаток бюджета / оставшиеся дни
        var todayLimit = daysRemaining > 0
            ? Math.Max(0, remainingMonthly / daysRemaining)
            : 0;

        var todayRemaining = todayLimit - todaySpent;

        // Лимит на завтра: пересчитывается с учётом итогов сегодняшнего дня
        var daysRemainingTomorrow = daysRemaining - 1;
        decimal tomorrowLimit = 0;
        string tomorrowReason;

        if (daysRemainingTomorrow > 0)
        {
            // После завершения сегодняшнего дня остаток = remainingMonthly - todaySpent
            var remainingAfterToday = remainingMonthly - todaySpent;
            tomorrowLimit = Math.Max(0, remainingAfterToday / daysRemainingTomorrow);

            if (todaySpent < todayLimit)
            {
                var saved = todayLimit - todaySpent;
                tomorrowReason = $"+{saved:N0} ₽ сэкономлено сегодня — перенесено на завтра";
            }
            else if (todaySpent > todayLimit)
            {
                var overspent = todaySpent - todayLimit;
                tomorrowReason = $"−{overspent:N0} ₽ перерасход сегодня — уменьшает лимит";
            }
            else
            {
                tomorrowReason = "Бюджет сегодня соблюдён точно";
            }
        }
        else
        {
            tomorrowReason = "Последний день месяца";
        }

        return new DailyBudgetStateDto(
            MonthlyBudget: plan.MonthlyAmount,
            TotalSpentThisMonth: totalSpent,
            RemainingMonthly: remainingMonthly,
            TodayLimit: todayLimit,
            TodaySpent: todaySpent,
            TodayRemaining: todayRemaining,
            TomorrowLimit: tomorrowLimit,
            TomorrowChangeReason: tomorrowReason,
            DaysRemainingInMonth: daysRemaining,
            Today: today
        );
    }

    public async Task<List<DailyStatsDto>> GetRecentDaysStatsAsync(Guid userId, int days = 7, CancellationToken ct = default)
    {
        var plan = await budgetRepo.GetCurrentAsync(userId, ct);
        if (plan is null) return [];

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var result = new List<DailyStatsDto>(days);

        // Получаем расходы за нужный диапазон одним запросом
        var from = today.AddDays(-(days - 1)).ToDateTime(TimeOnly.MinValue);
        var to = today.ToDateTime(TimeOnly.MaxValue);
        var expenses = await expenseRepo.GetByDateRangeAsync(plan.Id, from, to, ct);

        var totalSpentBeforeRange = await expenseRepo.GetTotalSpentBeforeDateAsync(
            plan.Id, today.AddDays(-(days - 1)), ct);

        var daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);
        // Для расчёта исторических лимитов: восстанавливаем лимит на каждый день
        // Используем упрощённый подход: лимит = оставшийся бюджет на тот момент / оставшиеся дни
        var runningSpent = totalSpentBeforeRange;

        for (int i = days - 1; i >= 0; i--)
        {
            var date = today.AddDays(-i);
            var daySpent = expenses
                .Where(e => DateOnly.FromDateTime(e.ExpenseDate) == date)
                .Sum(e => e.Amount);

            var daysRemainingOnDate = daysInMonth - date.Day + 1;
            var remainingBudgetOnDate = plan.MonthlyAmount - runningSpent;
            var limitOnDate = daysRemainingOnDate > 0
                ? Math.Max(0, remainingBudgetOnDate / daysRemainingOnDate)
                : 0;

            result.Add(new DailyStatsDto(
                Date: date,
                Spent: daySpent,
                Limit: limitOnDate,
                IsOverspent: daySpent > limitOnDate
            ));

            // Только для прошедших дней накапливаем фактические расходы
            if (date < today)
                runningSpent += daySpent;
        }

        return result;
    }

    private static BudgetPlanDto MapToDto(BudgetPlan plan) =>
        new(plan.Id, plan.Month, plan.Year, plan.MonthlyAmount, plan.CreatedAt);
}
