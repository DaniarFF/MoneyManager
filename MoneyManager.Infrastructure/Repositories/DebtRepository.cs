using Microsoft.EntityFrameworkCore;
using MoneyManager.Domain.Entities;
using MoneyManager.Domain.Interfaces;
using MoneyManager.Infrastructure.Persistence;

namespace MoneyManager.Infrastructure.Repositories;

public class DebtRepository(AppDbContext db) : IDebtRepository
{
    public Task<List<Debt>> GetByUserAsync(Guid userId, CancellationToken ct = default) =>
        db.Debts
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

    public Task<Debt?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.Debts.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(Debt debt, CancellationToken ct = default) =>
        await db.Debts.AddAsync(debt, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        db.SaveChangesAsync(ct);
}
