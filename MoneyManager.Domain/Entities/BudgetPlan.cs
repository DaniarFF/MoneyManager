namespace MoneyManager.Domain.Entities;

public class BudgetPlan
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public int Month { get; private set; }
    public int Year { get; private set; }
    public decimal MonthlyAmount { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public User User { get; private set; } = default!;

    private readonly List<Expense> _expenses = new();
    public IReadOnlyCollection<Expense> Expenses => _expenses.AsReadOnly();

    private BudgetPlan() { }

    public static BudgetPlan Create(Guid userId, int month, int year, decimal monthlyAmount)
    {
        if (month < 1 || month > 12) throw new ArgumentOutOfRangeException(nameof(month));
        if (year < 2000) throw new ArgumentOutOfRangeException(nameof(year));
        if (monthlyAmount <= 0) throw new ArgumentOutOfRangeException(nameof(monthlyAmount), "Monthly amount must be positive.");

        return new BudgetPlan
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Month = month,
            Year = year,
            MonthlyAmount = monthlyAmount,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateAmount(decimal newAmount)
    {
        if (newAmount <= 0) throw new ArgumentOutOfRangeException(nameof(newAmount));
        MonthlyAmount = newAmount;
    }
}
