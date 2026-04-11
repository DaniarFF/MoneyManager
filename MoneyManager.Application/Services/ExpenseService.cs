using MoneyManager.Application.Commands;
using MoneyManager.Application.DTOs;
using MoneyManager.Domain.Entities;
using MoneyManager.Domain.Interfaces;

namespace MoneyManager.Application.Services;

public class ExpenseService(
    IBudgetPlanRepository budgetRepo,
    IExpenseRepository expenseRepo) : IExpenseService
{
    public async Task<ExpenseDto> AddExpenseAsync(AddExpenseCommand cmd, CancellationToken ct = default)
    {
        var expense = Expense.Create(cmd.BudgetPlanId, cmd.Amount, cmd.Category, cmd.Note, cmd.ExpenseDate);
        await expenseRepo.AddAsync(expense, ct);
        await expenseRepo.SaveChangesAsync(ct);
        return MapToDto(expense);
    }

    public async Task UpdateExpenseAsync(UpdateExpenseCommand cmd, CancellationToken ct = default)
    {
        var expense = await expenseRepo.GetByIdAsync(cmd.ExpenseId, ct)
            ?? throw new InvalidOperationException($"Expense {cmd.ExpenseId} not found.");

        expense.Update(cmd.Amount, cmd.Category, cmd.Note, cmd.ExpenseDate);
        await expenseRepo.SaveChangesAsync(ct);
    }

    public async Task DeleteExpenseAsync(Guid expenseId, CancellationToken ct = default)
    {
        var expense = await expenseRepo.GetByIdAsync(expenseId, ct)
            ?? throw new InvalidOperationException($"Expense {expenseId} not found.");

        await expenseRepo.DeleteAsync(expense, ct);
        await expenseRepo.SaveChangesAsync(ct);
    }

    public async Task<List<ExpenseDto>> GetExpensesForCurrentMonthAsync(Guid userId, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var plan = await budgetRepo.GetByUserAndMonthAsync(userId, now.Month, now.Year, ct);
        if (plan is null) return [];

        var expenses = await expenseRepo.GetByBudgetPlanAsync(plan.Id, ct);
        return expenses.OrderByDescending(e => e.ExpenseDate).Select(MapToDto).ToList();
    }

    public async Task<List<ExpenseDto>> GetExpensesByDateAsync(Guid userId, DateOnly date, CancellationToken ct = default)
    {
        var plan = await budgetRepo.GetByUserAndMonthAsync(userId, date.Month, date.Year, ct);
        if (plan is null) return [];

        var from = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var to   = date.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);
        var expenses = await expenseRepo.GetByDateRangeAsync(plan.Id, from, to, ct);
        return expenses.OrderByDescending(e => e.ExpenseDate).Select(MapToDto).ToList();
    }

    private static ExpenseDto MapToDto(Expense e) =>
        new(e.Id, e.Amount, e.Category, e.Note, e.ExpenseDate, e.CreatedAt);
}
