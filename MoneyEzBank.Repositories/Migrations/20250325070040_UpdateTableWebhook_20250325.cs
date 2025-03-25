using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyEzBank.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableWebhook_20250325 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WebhookConfigs_Url_Type",
                table: "WebhookConfigs");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "WebhookConfigs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "WebhookConfigs",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_WebhookConfigs_Url_Type",
                table: "WebhookConfigs",
                columns: new[] { "Url", "Type" },
                unique: true);
        }
    }
}
