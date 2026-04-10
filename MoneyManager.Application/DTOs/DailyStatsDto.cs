namespace MoneyManager.Application.DTOs;

public record DailyStatsDto(
    DateOnly Date,
    decimal Spent,
    decimal Limit,
    bool IsOverspent
);
