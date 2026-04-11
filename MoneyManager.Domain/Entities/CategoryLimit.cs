namespace MoneyManager.Domain.Entities;

public class CategoryLimit
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Name { get; private set; } = default!;
    public string? Icon { get; private set; }
    public string Color { get; private set; } = "#007AFF"; // hex цвет карточки
    public decimal LimitAmount { get; private set; }
    public DateTime PeriodStart { get; private set; }
    public DateTime PeriodEnd { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsArchived { get; private set; }

    private CategoryLimit() { }

    public static CategoryLimit Create(Guid userId, string name, string? icon, string color, decimal limitAmount)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (limitAmount <= 0) throw new ArgumentException("Сумма лимита должна быть больше нуля.");

        var now = DateTime.UtcNow;
        return new CategoryLimit
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = name.Trim(),
            Icon = icon?.Trim(),
            Color = string.IsNullOrWhiteSpace(color) ? "#007AFF" : color.Trim(),
            LimitAmount = limitAmount,
            PeriodStart = now,
            PeriodEnd = now.AddMonths(1),
            CreatedAt = now,
            IsArchived = false
        };
    }

    public void Update(string name, string? icon, string color, decimal limitAmount)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (limitAmount <= 0) throw new ArgumentException("Сумма лимита должна быть больше нуля.");
        Name = name.Trim();
        Icon = icon?.Trim();
        Color = string.IsNullOrWhiteSpace(color) ? "#007AFF" : color.Trim();
        LimitAmount = limitAmount;
    }

    public void Archive() => IsArchived = true;
}
