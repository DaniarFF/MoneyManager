using MoneyManager.Domain.Entities;

namespace MoneyManager.Domain.Interfaces;

public interface IDebtRepository
{
    Task<List<Debt>> GetByUserAsync(Guid userId, CancellationToken ct = default);
    Task<Debt?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Debt debt, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
