using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddColorToCategoryLimit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "color",
                table: "category_limits",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "#007AFF");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "color",
                table: "category_limits");
        }
    }
}
