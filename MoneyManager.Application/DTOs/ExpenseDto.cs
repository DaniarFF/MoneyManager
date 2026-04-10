namespace MoneyManager.Application.DTOs;

public record ExpenseDto(
    Guid Id,
    decimal Amount,
    string Category,
    string? Note,
    DateTime ExpenseDate,
    DateTime CreatedAt
);
