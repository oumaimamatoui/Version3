using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoEvaluation.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class FinalConsolidation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Campagnes_Utilisateurs_PersonnelId",
                table: "Campagnes");

            migrationBuilder.DropTable(
                name: "EntrepriseParSA");

            migrationBuilder.DropIndex(
                name: "IX_Campagnes_PersonnelId",
                table: "Campagnes");

            migrationBuilder.DropColumn(
                name: "UserType",
                table: "Utilisateurs");

            migrationBuilder.DropColumn(
                name: "PersonnelId",
                table: "Campagnes");

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateurs_Email",
                table: "Utilisateurs",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Utilisateurs_Email",
                table: "Utilisateurs");

            migrationBuilder.AddColumn<string>(
                name: "UserType",
                table: "Utilisateurs",
                type: "character varying(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "PersonnelId",
                table: "Campagnes",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EntrepriseParSA",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EntrepriseId = table.Column<Guid>(type: "uuid", nullable: true),
                    Adresse = table.Column<string>(type: "text", nullable: true),
                    CodePostal = table.Column<string>(type: "text", nullable: true),
                    CreeLe = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Domaine = table.Column<string>(type: "text", nullable: true),
                    EmailResponsable = table.Column<string>(type: "text", nullable: true),
                    Industrie = table.Column<string>(type: "text", nullable: true),
                    NomResponsable = table.Column<string>(type: "text", nullable: true),
                    Pays = table.Column<string>(type: "text", nullable: true),
                    PrenomResponsable = table.Column<string>(type: "text", nullable: true),
                    SiteWeb = table.Column<string>(type: "text", nullable: true),
                    TelephoneResponsable = table.Column<string>(type: "text", nullable: true),
                    Ville = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntrepriseParSA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntrepriseParSA_Entreprises_EntrepriseId",
                        column: x => x.EntrepriseId,
                        principalTable: "Entreprises",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Campagnes_PersonnelId",
                table: "Campagnes",
                column: "PersonnelId");

            migrationBuilder.CreateIndex(
                name: "IX_EntrepriseParSA_EntrepriseId",
                table: "EntrepriseParSA",
                column: "EntrepriseId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Campagnes_Utilisateurs_PersonnelId",
                table: "Campagnes",
                column: "PersonnelId",
                principalTable: "Utilisateurs",
                principalColumn: "Id");
        }
    }
}
