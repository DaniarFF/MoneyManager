namespace MoneyManager.Domain.Entities;

public enum DebtDirection { IOwe, TheyOwe }
public enum DebtStatus { Active, Paid }

public class Debt
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string PersonName { get; private set; } = default!;
    public DebtDirection Direction { get; private set; }
    public decimal Amount { get; private set; }
    public string? Description { get; private set; }
    public DebtStatus Status { get; private set; }
    public DateTime? DueDate { get; private set; }
    public Guid? LinkedExpenseId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? PaidAt { get; private set; }

    private Debt() { }

    public static Debt Create(
        Guid userId,
        string personName,
        DebtDirection direction,
        decimal amount,
        string? description,
        DateTime? dueDate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(personName);
        if (amount <= 0) throw new ArgumentException("Сумма долга должна быть больше нуля.");

        return new Debt
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            PersonName = personName.Trim(),
            Direction = direction,
            Amount = amount,
            Description = description?.Trim(),
            Status = DebtStatus.Active,
            DueDate = dueDate,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkAsPaid()
    {
        Status = DebtStatus.Paid;
        PaidAt = DateTime.UtcNow;
    }

    public void SetLinkedExpense(Guid expenseId) => LinkedExpenseId = expenseId;
}
