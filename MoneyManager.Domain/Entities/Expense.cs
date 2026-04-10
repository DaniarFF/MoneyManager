namespace MoneyManager.Domain.Entities;

public class Expense
{
    public Guid Id { get; private set; }
    public Guid BudgetPlanId { get; private set; }
    public decimal Amount { get; private set; }
    public string Category { get; private set; } = default!;
    public string? Note { get; private set; }
    public DateTime ExpenseDate { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public BudgetPlan BudgetPlan { get; private set; } = default!;

    private Expense() { }

    public static Expense Create(Guid budgetPlanId, decimal amount, string category, string? note, DateTime expenseDate)
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive.");
        ArgumentException.ThrowIfNullOrWhiteSpace(category);

        return new Expense
        {
            Id = Guid.NewGuid(),
            BudgetPlanId = budgetPlanId,
            Amount = amount,
            Category = category,
            Note = note,
            ExpenseDate = expenseDate,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(decimal amount, string category, string? note, DateTime expenseDate)
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
        ArgumentException.ThrowIfNullOrWhiteSpace(category);

        Amount = amount;
        Category = category;
        Note = note;
        ExpenseDate = expenseDate;
    }
}
