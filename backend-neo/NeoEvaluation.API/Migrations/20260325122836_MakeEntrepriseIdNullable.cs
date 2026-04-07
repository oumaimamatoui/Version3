using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoEvaluation.API.Migrations
{
    /// <inheritdoc />
    public partial class MakeEntrepriseIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Campagnes_CampagneId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumentsCandidats_Utilisateurs_CandidatId",
                table: "DocumentsCandidats");

            migrationBuilder.DropForeignKey(
                name: "FK_Utilisateurs_Entreprises_EntrepriseId",
                table: "Utilisateurs");

            migrationBuilder.DropTable(
                name: "Invitations");

            migrationBuilder.DropTable(
                name: "Plannings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DocumentsCandidats",
                table: "DocumentsCandidats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Documents",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "NomEntreprise",
                table: "Utilisateurs");

            migrationBuilder.RenameTable(
                name: "DocumentsCandidats",
                newName: "DocumentCandidat");

            migrationBuilder.RenameTable(
                name: "Documents",
                newName: "Document");

            migrationBuilder.RenameIndex(
                name: "IX_DocumentsCandidats_CandidatId",
                table: "DocumentCandidat",
                newName: "IX_DocumentCandidat_CandidatId");

            migrationBuilder.RenameIndex(
                name: "IX_Documents_CampagneId",
                table: "Document",
                newName: "IX_Document_CampagneId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DocumentCandidat",
                table: "DocumentCandidat",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Document",
                table: "Document",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Document_Campagnes_CampagneId",
                table: "Document",
                column: "CampagneId",
                principalTable: "Campagnes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentCandidat_Utilisateurs_CandidatId",
                table: "DocumentCandidat",
                column: "CandidatId",
                principalTable: "Utilisateurs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Utilisateurs_Entreprises_EntrepriseId",
                table: "Utilisateurs",
                column: "EntrepriseId",
                principalTable: "Entreprises",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Document_Campagnes_CampagneId",
                table: "Document");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumentCandidat_Utilisateurs_CandidatId",
                table: "DocumentCandidat");

            migrationBuilder.DropForeignKey(
                name: "FK_Utilisateurs_Entreprises_EntrepriseId",
                table: "Utilisateurs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DocumentCandidat",
                table: "DocumentCandidat");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Document",
                table: "Document");

            migrationBuilder.RenameTable(
                name: "DocumentCandidat",
                newName: "DocumentsCandidats");

            migrationBuilder.RenameTable(
                name: "Document",
                newName: "Documents");

            migrationBuilder.RenameIndex(
                name: "IX_DocumentCandidat_CandidatId",
                table: "DocumentsCandidats",
                newName: "IX_DocumentsCandidats_CandidatId");

            migrationBuilder.RenameIndex(
                name: "IX_Document_CampagneId",
                table: "Documents",
                newName: "IX_Documents_CampagneId");

            migrationBuilder.AddColumn<string>(
                name: "NomEntreprise",
                table: "Utilisateurs",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DocumentsCandidats",
                table: "DocumentsCandidats",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Documents",
                table: "Documents",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Invitations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CampagneId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateEnvoi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Statut = table.Column<string>(type: "text", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invitations", x => x.Id);
                });

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

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Campagnes_CampagneId",
                table: "Documents",
                column: "CampagneId",
                principalTable: "Campagnes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentsCandidats_Utilisateurs_CandidatId",
                table: "DocumentsCandidats",
                column: "CandidatId",
                principalTable: "Utilisateurs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Utilisateurs_Entreprises_EntrepriseId",
                table: "Utilisateurs",
                column: "EntrepriseId",
                principalTable: "Entreprises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
