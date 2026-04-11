using MoneyManager.Application.Commands;
using MoneyManager.Application.DTOs;
using MoneyManager.Domain.Entities;
using MoneyManager.Domain.Interfaces;

namespace MoneyManager.Application.Services;

public class DebtService(
    IDebtRepository debtRepo,
    IExpenseRepository expenseRepo) : IDebtService
{
    public async Task<DebtDto> CreateAsync(CreateDebtCommand cmd, CancellationToken ct = default)
    {
        var debt = Debt.Create(
            cmd.UserId, cmd.PersonName, cmd.Direction,
            cmd.Amount, cmd.Description, cmd.DueDate);

        // Опционально списываем с бюджета (только для IOwe)
        if (cmd.Direction == DebtDirection.IOwe
            && cmd.DeductFromBudgetPlanId.HasValue
            && !string.IsNullOrWhiteSpace(cmd.DeductCategory))
        {
            var expense = Expense.Create(
                cmd.DeductFromBudgetPlanId.Value,
                cmd.Amount,
                cmd.DeductCategory,
                $"Долг: {cmd.PersonName}" + (cmd.Description != null ? $" — {cmd.Description}" : ""),
                DateTime.UtcNow,
                isIncome: false);

            await expenseRepo.AddAsync(expense, ct);
            await expenseRepo.SaveChangesAsync(ct);
            debt.SetLinkedExpense(expense.Id);
        }

        await debtRepo.AddAsync(debt, ct);
        await debtRepo.SaveChangesAsync(ct);

        return MapToDto(debt);
    }

    public async Task<List<DebtDto>> GetAllAsync(Guid userId, CancellationToken ct = default)
    {
        var debts = await debtRepo.GetByUserAsync(userId, ct);
        return debts.Select(MapToDto).ToList();
    }

    public async Task MarkAsPaidAsync(Guid debtId, CancellationToken ct = default)
    {
        var debt = await debtRepo.GetByIdAsync(debtId, ct)
            ?? throw new InvalidOperationException($"Debt {debtId} not found.");
        debt.MarkAsPaid();
        await debtRepo.SaveChangesAsync(ct);
    }

    private static DebtDto MapToDto(Debt d)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var isOverdue = d.Status == DebtStatus.Active
                     && d.DueDate.HasValue
                     && DateOnly.FromDateTime(d.DueDate.Value) < today;

        return new DebtDto(
            d.Id,
            d.PersonName,
            d.Direction.ToString(),
            d.Amount,
            d.Description,
            d.Status.ToString(),
            d.DueDate,
            d.CreatedAt,
            d.PaidAt,
            isOverdue);
    }
}
