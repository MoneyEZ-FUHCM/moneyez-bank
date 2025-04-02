using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyEzBank.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountHolder",
                table: "Accounts",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountHolder",
                table: "Accounts");
        }
    }
}
