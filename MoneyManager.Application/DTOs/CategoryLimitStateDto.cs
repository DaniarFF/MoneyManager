namespace MoneyManager.Application.DTOs;

public record CategoryLimitStateDto(
    Guid LimitId,
    string Name,
    string? Icon,
    string Color,
    decimal LimitAmount,
    decimal TotalSpent,
    decimal Remaining,
    decimal TodaySpent,
    decimal TodayLimit,
    decimal TodayRemaining,
    decimal TomorrowLimit,
    string TomorrowReason,
    int DaysRemaining,
    DateOnly PeriodEndDate,
    List<DailyStatsDto> RecentDays
);
