using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoEvaluation.API.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestionTimer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DureeSecondes",
                table: "Questions",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DureeSecondes",
                table: "Questions");
        }
    }
}
