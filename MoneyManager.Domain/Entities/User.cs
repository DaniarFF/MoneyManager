namespace MoneyManager.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string AccessKey { get; private set; } = default!; // hashed passphrase
    public string DisplayName { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }

    private readonly List<BudgetPlan> _budgetPlans = new();
    public IReadOnlyCollection<BudgetPlan> BudgetPlans => _budgetPlans.AsReadOnly();

    private User() { }

    public static User Create(string displayName, string hashedAccessKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        ArgumentException.ThrowIfNullOrWhiteSpace(hashedAccessKey);

        return new User
        {
            Id = Guid.NewGuid(),
            DisplayName = displayName,
            AccessKey = hashedAccessKey,
            CreatedAt = DateTime.UtcNow
        };
    }
}
