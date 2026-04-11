using MoneyManager.Domain.Entities;

namespace MoneyManager.Domain.Interfaces;

public interface ICategoryLimitRepository
{
    Task<List<CategoryLimit>> GetActiveByUserAsync(Guid userId, CancellationToken ct = default);
    Task<CategoryLimit?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(CategoryLimit limit, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
