using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyEzBank.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableWebhook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "WebhookConfigs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "WebhookConfigs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "FailureCount",
                table: "WebhookConfigs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "WebhookConfigs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastFailureAt",
                table: "WebhookConfigs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxRetries",
                table: "WebhookConfigs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RetryIntervalSeconds",
                table: "WebhookConfigs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_WebhookConfigs_AccountId",
                table: "WebhookConfigs",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_WebhookConfigs_Accounts_AccountId",
                table: "WebhookConfigs",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WebhookConfigs_Accounts_AccountId",
                table: "WebhookConfigs");

            migrationBuilder.DropIndex(
                name: "IX_WebhookConfigs_AccountId",
                table: "WebhookConfigs");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "WebhookConfigs");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "WebhookConfigs");

            migrationBuilder.DropColumn(
                name: "FailureCount",
                table: "WebhookConfigs");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "WebhookConfigs");

            migrationBuilder.DropColumn(
                name: "LastFailureAt",
                table: "WebhookConfigs");

            migrationBuilder.DropColumn(
                name: "MaxRetries",
                table: "WebhookConfigs");

            migrationBuilder.DropColumn(
                name: "RetryIntervalSeconds",
                table: "WebhookConfigs");
        }
    }
}
