namespace MoneyManager.Application.Commands;

public record CreateBudgetPlanCommand(
    Guid UserId,
    int Month,
    int Year,
    decimal MonthlyAmount
);
