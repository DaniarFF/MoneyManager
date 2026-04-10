using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoneyManager.Domain.Entities;

namespace MoneyManager.Infrastructure.Persistence.Configurations;

public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.ToTable("expenses");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.BudgetPlanId).HasColumnName("budget_plan_id");
        builder.Property(e => e.Amount).HasColumnName("amount").HasColumnType("numeric(18,2)");
        builder.Property(e => e.Category).HasColumnName("category").HasMaxLength(100).IsRequired();
        builder.Property(e => e.Note).HasColumnName("note").HasMaxLength(500);
        builder.Property(e => e.ExpenseDate).HasColumnName("expense_date");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at");

        builder.HasOne(e => e.BudgetPlan)
            .WithMany(b => b.Expenses)
            .HasForeignKey(e => e.BudgetPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        // Индекс для быстрых запросов по дате
        builder.HasIndex(e => new { e.BudgetPlanId, e.ExpenseDate });
    }
}
