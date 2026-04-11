using MoneyManager.Domain.Entities;

namespace MoneyManager.Domain.Interfaces;

public interface IExpenseRepository
{
    Task<Expense?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Expense>> GetByBudgetPlanAsync(Guid budgetPlanId, CancellationToken ct = default);
    Task<List<Expense>> GetByDateRangeAsync(Guid budgetPlanId, DateTime from, DateTime to, CancellationToken ct = default);
    /// <summary>Расходы по пользователю в диапазоне дат и категории (для категорийных лимитов).</summary>
    Task<List<Expense>> GetByUserCategoryAndPeriodAsync(Guid userId, string category, DateTime from, DateTime to, CancellationToken ct = default);
    Task<decimal> GetTotalSpentAsync(Guid budgetPlanId, CancellationToken ct = default);
    Task<decimal> GetSpentOnDateAsync(Guid budgetPlanId, DateOnly date, CancellationToken ct = default);
    /// <summary>
    /// Сумма расходов до (не включая) указанной даты — для расчёта исторических лимитов.
    /// </summary>
    Task<decimal> GetTotalSpentBeforeDateAsync(Guid budgetPlanId, DateOnly beforeDate, CancellationToken ct = default);
    Task AddAsync(Expense expense, CancellationToken ct = default);
    Task DeleteAsync(Expense expense, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
