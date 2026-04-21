using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoEvaluation.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGmailOAuthToEntreprise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GmailAccessToken",
                table: "Entreprises",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GmailEmail",
                table: "Entreprises",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GmailRefreshToken",
                table: "Entreprises",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GmailScope",
                table: "Entreprises",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GmailTokenExpiresAt",
                table: "Entreprises",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GmailAccessToken",
                table: "Entreprises");

            migrationBuilder.DropColumn(
                name: "GmailEmail",
                table: "Entreprises");

            migrationBuilder.DropColumn(
                name: "GmailRefreshToken",
                table: "Entreprises");

            migrationBuilder.DropColumn(
                name: "GmailScope",
                table: "Entreprises");

            migrationBuilder.DropColumn(
                name: "GmailTokenExpiresAt",
                table: "Entreprises");
        }
    }
}
