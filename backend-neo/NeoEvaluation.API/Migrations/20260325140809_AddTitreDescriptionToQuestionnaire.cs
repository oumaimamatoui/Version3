using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoEvaluation.API.Migrations
{
    /// <inheritdoc />
    public partial class AddTitreDescriptionToQuestionnaire : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Difficulte",
                table: "Questionnaires",
                newName: "Description");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreeLe",
                table: "Questionnaires",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreeLe",
                table: "Questionnaires");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Questionnaires",
                newName: "Difficulte");
        }
    }
}
