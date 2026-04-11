namespace MoneyManager.Application.Commands;

public record CreateCategoryLimitCommand(
    Guid UserId,
    string Name,
    string? Icon,
    string Color,
    decimal LimitAmount
);
