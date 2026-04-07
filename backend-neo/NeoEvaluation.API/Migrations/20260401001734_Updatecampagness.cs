using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoEvaluation.API.Migrations
{
    /// <inheritdoc />
    public partial class Updatecampagness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Categorie",
                table: "Campagnes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Campagnes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DureeMinutes",
                table: "Campagnes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NbTentativesMax",
                table: "Campagnes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NiveauDifficulte",
                table: "Campagnes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ScorePassage",
                table: "Campagnes",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Categorie",
                table: "Campagnes");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Campagnes");

            migrationBuilder.DropColumn(
                name: "DureeMinutes",
                table: "Campagnes");

            migrationBuilder.DropColumn(
                name: "NbTentativesMax",
                table: "Campagnes");

            migrationBuilder.DropColumn(
                name: "NiveauDifficulte",
                table: "Campagnes");

            migrationBuilder.DropColumn(
                name: "ScorePassage",
                table: "Campagnes");
        }
    }
}
