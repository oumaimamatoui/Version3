using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoEvaluation.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanningsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Plannings");
        }
    }
}
