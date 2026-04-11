using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoneyManager.Domain.Entities;

namespace MoneyManager.Infrastructure.Persistence.Configurations;

public class CategoryLimitConfiguration : IEntityTypeConfiguration<CategoryLimit>
{
    public void Configure(EntityTypeBuilder<CategoryLimit> builder)
    {
        builder.ToTable("category_limits");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Icon).HasColumnName("icon").HasMaxLength(20);
        builder.Property(x => x.Color).HasColumnName("color").HasMaxLength(20).HasDefaultValue("#007AFF");
        builder.Property(x => x.LimitAmount).HasColumnName("limit_amount").HasColumnType("numeric(18,2)");
        builder.Property(x => x.PeriodStart).HasColumnName("period_start");
        builder.Property(x => x.PeriodEnd).HasColumnName("period_end");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.IsArchived).HasColumnName("is_archived").HasDefaultValue(false);

        builder.HasIndex(x => new { x.UserId, x.IsArchived });

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
