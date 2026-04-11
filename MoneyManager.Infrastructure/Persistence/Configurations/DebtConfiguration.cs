using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoneyManager.Domain.Entities;

namespace MoneyManager.Infrastructure.Persistence.Configurations;

public class DebtConfiguration : IEntityTypeConfiguration<Debt>
{
    public void Configure(EntityTypeBuilder<Debt> builder)
    {
        builder.ToTable("debts");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.Property(x => x.PersonName).HasColumnName("person_name").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Direction).HasColumnName("direction");
        builder.Property(x => x.Amount).HasColumnName("amount").HasColumnType("numeric(18,2)");
        builder.Property(x => x.Description).HasColumnName("description").HasMaxLength(500);
        builder.Property(x => x.Status).HasColumnName("status");
        builder.Property(x => x.DueDate).HasColumnName("due_date");
        builder.Property(x => x.LinkedExpenseId).HasColumnName("linked_expense_id");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.PaidAt).HasColumnName("paid_at");

        builder.HasIndex(x => new { x.UserId, x.Status });

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
