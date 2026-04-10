namespace MoneyManager.Application.DTOs;

public record BudgetPlanDto(
    Guid Id,
    int Month,
    int Year,
    decimal MonthlyAmount,
    DateTime CreatedAt
);
