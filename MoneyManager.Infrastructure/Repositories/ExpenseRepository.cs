using Microsoft.EntityFrameworkCore;
using MoneyManager.Domain.Entities;
using MoneyManager.Domain.Interfaces;
using MoneyManager.Infrastructure.Persistence;

namespace MoneyManager.Infrastructure.Repositories;

public class ExpenseRepository(AppDbContext db) : IExpenseRepository
{
    public Task<Expense?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.Expenses.FirstOrDefaultAsync(e => e.Id == id, ct);

    public Task<List<Expense>> GetByBudgetPlanAsync(Guid budgetPlanId, CancellationToken ct = default) =>
        db.Expenses
            .Where(e => e.BudgetPlanId == budgetPlanId)
            .OrderByDescending(e => e.ExpenseDate)
            .ToListAsync(ct);

    public Task<List<Expense>> GetByDateRangeAsync(Guid budgetPlanId, DateTime from, DateTime to, CancellationToken ct = default) =>
        db.Expenses
            .Where(e => e.BudgetPlanId == budgetPlanId && e.ExpenseDate >= from && e.ExpenseDate <= to)
            .OrderByDescending(e => e.ExpenseDate)
            .ToListAsync(ct);

    public async Task<decimal> GetTotalSpentAsync(Guid budgetPlanId, CancellationToken ct = default) =>
        await db.Expenses
            .Where(e => e.BudgetPlanId == budgetPlanId)
            .SumAsync(e => e.Amount, ct);

    public async Task<decimal> GetSpentOnDateAsync(Guid budgetPlanId, DateOnly date, CancellationToken ct = default)
    {
        var from = date.ToDateTime(TimeOnly.MinValue);
        var to = date.ToDateTime(TimeOnly.MaxValue);
        return await db.Expenses
            .Where(e => e.BudgetPlanId == budgetPlanId && e.ExpenseDate >= from && e.ExpenseDate <= to)
            .SumAsync(e => e.Amount, ct);
    }

    public async Task<decimal> GetTotalSpentBeforeDateAsync(Guid budgetPlanId, DateOnly beforeDate, CancellationToken ct = default)
    {
        var cutoff = beforeDate.ToDateTime(TimeOnly.MinValue);
        return await db.Expenses
            .Where(e => e.BudgetPlanId == budgetPlanId && e.ExpenseDate < cutoff)
            .SumAsync(e => e.Amount, ct);
    }

    public async Task AddAsync(Expense expense, CancellationToken ct = default) =>
        await db.Expenses.AddAsync(expense, ct);

    public Task DeleteAsync(Expense expense, CancellationToken ct = default)
    {
        db.Expenses.Remove(expense);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        db.SaveChangesAsync(ct);
}
