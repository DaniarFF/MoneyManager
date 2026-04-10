using MoneyManager.Domain.Entities;

namespace MoneyManager.Domain.Interfaces;

public interface IBudgetPlanRepository
{
    Task<BudgetPlan?> GetByUserAndMonthAsync(Guid userId, int month, int year, CancellationToken ct = default);
    Task<BudgetPlan?> GetCurrentAsync(Guid userId, CancellationToken ct = default);
    Task AddAsync(BudgetPlan plan, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
