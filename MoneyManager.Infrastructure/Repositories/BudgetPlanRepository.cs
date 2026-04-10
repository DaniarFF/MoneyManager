using Microsoft.EntityFrameworkCore;
using MoneyManager.Domain.Entities;
using MoneyManager.Domain.Interfaces;
using MoneyManager.Infrastructure.Persistence;

namespace MoneyManager.Infrastructure.Repositories;

public class BudgetPlanRepository(AppDbContext db) : IBudgetPlanRepository
{
    public Task<BudgetPlan?> GetByUserAndMonthAsync(Guid userId, int month, int year, CancellationToken ct = default) =>
        db.BudgetPlans
            .FirstOrDefaultAsync(b => b.UserId == userId && b.Month == month && b.Year == year, ct);

    public Task<BudgetPlan?> GetCurrentAsync(Guid userId, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        return db.BudgetPlans
            .FirstOrDefaultAsync(b => b.UserId == userId && b.Month == now.Month && b.Year == now.Year, ct);
    }

    public async Task AddAsync(BudgetPlan plan, CancellationToken ct = default) =>
        await db.BudgetPlans.AddAsync(plan, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        db.SaveChangesAsync(ct);
}
