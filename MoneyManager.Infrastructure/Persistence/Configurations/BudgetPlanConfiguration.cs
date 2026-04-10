using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoneyManager.Domain.Entities;

namespace MoneyManager.Infrastructure.Persistence.Configurations;

public class BudgetPlanConfiguration : IEntityTypeConfiguration<BudgetPlan>
{
    public void Configure(EntityTypeBuilder<BudgetPlan> builder)
    {
        builder.ToTable("budget_plans");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id).HasColumnName("id");
        builder.Property(b => b.UserId).HasColumnName("user_id");
        builder.Property(b => b.Month).HasColumnName("month");
        builder.Property(b => b.Year).HasColumnName("year");
        builder.Property(b => b.MonthlyAmount).HasColumnName("monthly_amount").HasColumnType("numeric(18,2)");
        builder.Property(b => b.CreatedAt).HasColumnName("created_at");

        builder.HasOne(b => b.User)
            .WithMany(u => u.BudgetPlans)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Уникальный план на один месяц для одного пользователя
        builder.HasIndex(b => new { b.UserId, b.Year, b.Month }).IsUnique();
    }
}
