using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MoneyManager.Domain.Entities;
using MoneyManager.Infrastructure.Persistence.Configurations;

namespace MoneyManager.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IDataProtectionKeyContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<BudgetPlan> BudgetPlans => Set<BudgetPlan>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<CategoryLimit> CategoryLimits => Set<CategoryLimit>();
    public DbSet<Debt> Debts => Set<Debt>();
    public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new BudgetPlanConfiguration());
        modelBuilder.ApplyConfiguration(new ExpenseConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryLimitConfiguration());
        modelBuilder.ApplyConfiguration(new DebtConfiguration());
    }
}
