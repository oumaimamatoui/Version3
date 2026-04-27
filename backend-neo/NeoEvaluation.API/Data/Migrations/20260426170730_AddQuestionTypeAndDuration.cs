using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoEvaluation.API.Data.Migrations
{
    public partial class AddQuestionTypeAndDuration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // On commente car ces colonnes sont déjà créées dans FreshStart
            /*
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Questions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DureeSecondes",
                table: "Questions",
                type: "integer",
                nullable: true);
            */
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            /*
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "DureeSecondes",
                table: "Questions");
            */
        }
    }
}