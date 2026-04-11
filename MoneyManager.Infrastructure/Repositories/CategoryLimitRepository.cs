using Microsoft.EntityFrameworkCore;
using MoneyManager.Domain.Entities;
using MoneyManager.Domain.Interfaces;
using MoneyManager.Infrastructure.Persistence;

namespace MoneyManager.Infrastructure.Repositories;

public class CategoryLimitRepository(AppDbContext db) : ICategoryLimitRepository
{
    public Task<List<CategoryLimit>> GetActiveByUserAsync(Guid userId, CancellationToken ct = default) =>
        db.CategoryLimits
            .Where(x => x.UserId == userId && !x.IsArchived)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(ct);

    public Task<CategoryLimit?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.CategoryLimits.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(CategoryLimit limit, CancellationToken ct = default) =>
        await db.CategoryLimits.AddAsync(limit, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        db.SaveChangesAsync(ct);
}
