namespace MoneyManager.Application.DTOs;

public record DebtDto(
    Guid Id,
    string PersonName,
    string Direction,   // "IOwe" | "TheyOwe"
    decimal Amount,
    string? Description,
    string Status,      // "Active" | "Paid"
    DateTime? DueDate,
    DateTime CreatedAt,
    DateTime? PaidAt,
    bool IsOverdue
);
