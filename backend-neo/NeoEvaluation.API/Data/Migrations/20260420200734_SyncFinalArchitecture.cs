using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoEvaluation.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class SyncFinalArchitecture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidatures_Entreprises_EntrepriseId",
                table: "Candidatures");

            migrationBuilder.DropIndex(
                name: "IX_Candidatures_EntrepriseId",
                table: "Candidatures");

            migrationBuilder.DropColumn(
                name: "EntrepriseId",
                table: "Candidatures");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EntrepriseId",
                table: "Candidatures",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candidatures_EntrepriseId",
                table: "Candidatures",
                column: "EntrepriseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidatures_Entreprises_EntrepriseId",
                table: "Candidatures",
                column: "EntrepriseId",
                principalTable: "Entreprises",
                principalColumn: "Id");
        }
    }
}
