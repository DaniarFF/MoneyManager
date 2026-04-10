using MoneyManager.Application.Commands;
using MoneyManager.Application.DTOs;

namespace MoneyManager.Application.Services;

public interface IExpenseService
{
    Task<ExpenseDto> AddExpenseAsync(AddExpenseCommand command, CancellationToken ct = default);
    Task UpdateExpenseAsync(UpdateExpenseCommand command, CancellationToken ct = default);
    Task DeleteExpenseAsync(Guid expenseId, CancellationToken ct = default);
    Task<List<ExpenseDto>> GetExpensesForCurrentMonthAsync(Guid userId, CancellationToken ct = default);
    Task<List<ExpenseDto>> GetExpensesByDateAsync(Guid userId, DateOnly date, CancellationToken ct = default);
}
