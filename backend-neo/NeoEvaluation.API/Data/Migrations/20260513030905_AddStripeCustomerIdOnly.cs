using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoEvaluation.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStripeCustomerIdOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Plan",
                table: "InscriptionsEntreprises");

            migrationBuilder.DropColumn(
                name: "IsUsageSuspended",
                table: "Entreprises");

            migrationBuilder.DropColumn(
                name: "LastUsageReset",
                table: "Entreprises");

            migrationBuilder.DropColumn(
                name: "MaxUsageLimit",
                table: "Entreprises");

            migrationBuilder.DropColumn(
                name: "UsageCount",
                table: "Entreprises");

            migrationBuilder.AddColumn<string>(
                name: "StripeCustomerId",
                table: "Entreprises",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StripeCustomerId",
                table: "Entreprises");

            migrationBuilder.AddColumn<string>(
                name: "Plan",
                table: "InscriptionsEntreprises",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsUsageSuspended",
                table: "Entreprises",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUsageReset",
                table: "Entreprises",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "MaxUsageLimit",
                table: "Entreprises",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsageCount",
                table: "Entreprises",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
