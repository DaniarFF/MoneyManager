namespace MoneyManager.Application.Commands;

public record UpdateExpenseCommand(
    Guid ExpenseId,
    decimal Amount,
    string Category,
    string? Note,
    DateTime ExpenseDate,
    bool IsIncome = false
);
