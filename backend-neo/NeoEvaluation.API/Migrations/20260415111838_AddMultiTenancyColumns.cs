using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoEvaluation.API.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiTenancyColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EntrepriseId",
                table: "Roles",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EntrepriseId",
                table: "Questions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EntrepriseId",
                table: "Questionnaires",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntrepriseId",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "EntrepriseId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "EntrepriseId",
                table: "Questionnaires");
        }
    }
}
