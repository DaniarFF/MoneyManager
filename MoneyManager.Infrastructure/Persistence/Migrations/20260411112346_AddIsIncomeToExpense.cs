using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIsIncomeToExpense : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_income",
                table: "expenses",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_income",
                table: "expenses");
        }
    }
}
