using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoEvaluation.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class MergePlanningIntoCampagne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Plannings");

            migrationBuilder.AddColumn<int>(
                name: "DureeMinutes",
                table: "Campagnes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ModeNotation",
                table: "Campagnes",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DureeMinutes",
                table: "Campagnes");

            migrationBuilder.DropColumn(
                name: "ModeNotation",
                table: "Campagnes");

            migrationBuilder.CreateTable(
                name: "Plannings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CampagneId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateOuverture = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DureeMinutes = table.Column<int>(type: "integer", nullable: false),
                    ModeNotation = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plannings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Plannings_Campagnes_CampagneId",
                        column: x => x.CampagneId,
                        principalTable: "Campagnes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Plannings_CampagneId",
                table: "Plannings",
                column: "CampagneId");
        }
    }
}
