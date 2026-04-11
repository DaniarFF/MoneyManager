namespace MoneyManager.Application.Commands;

public record AddExpenseCommand(
    Guid BudgetPlanId,
    decimal Amount,
    string Category,
    string? Note,
    DateTime ExpenseDate,
    bool IsIncome = false
);
