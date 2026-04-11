using MoneyManager.Domain.Entities;

namespace MoneyManager.Application.Commands;

public record CreateDebtCommand(
    Guid UserId,
    string PersonName,
    DebtDirection Direction,
    decimal Amount,
    string? Description,
    DateTime? DueDate,
    // Опциональное списание с бюджета (только для IOwe)
    Guid? DeductFromBudgetPlanId,
    string? DeductCategory
);
