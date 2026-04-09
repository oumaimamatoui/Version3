using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoEvaluation.API.Migrations
{
    /// <inheritdoc />
    public partial class CleanRefactorToEntrepriseParSA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Adresse",
                table: "InscriptionsEntreprises");

            migrationBuilder.DropColumn(
                name: "CodePostal",
                table: "InscriptionsEntreprises");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "InscriptionsEntreprises");

            migrationBuilder.DropColumn(
                name: "Domaine",
                table: "InscriptionsEntreprises");

            migrationBuilder.DropColumn(
                name: "Industrie",
                table: "InscriptionsEntreprises");

            migrationBuilder.DropColumn(
                name: "Pays",
                table: "InscriptionsEntreprises");

            migrationBuilder.DropColumn(
                name: "SiteWeb",
                table: "InscriptionsEntreprises");

            migrationBuilder.DropColumn(
                name: "Ville",
                table: "InscriptionsEntreprises");

            migrationBuilder.DropColumn(
                name: "Adresse",
                table: "Entreprises");

            migrationBuilder.DropColumn(
                name: "CodePostal",
                table: "Entreprises");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Entreprises");

            migrationBuilder.DropColumn(
                name: "Domaine",
                table: "Entreprises");

            migrationBuilder.DropColumn(
                name: "Industrie",
                table: "Entreprises");

            migrationBuilder.DropColumn(
                name: "Pays",
                table: "Entreprises");

            migrationBuilder.DropColumn(
                name: "SiteWeb",
                table: "Entreprises");

            migrationBuilder.DropColumn(
                name: "Ville",
                table: "Entreprises");

            migrationBuilder.CreateTable(
                name: "EntrepriseParSA",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EntrepriseId = table.Column<Guid>(type: "uuid", nullable: true),
                    Domaine = table.Column<string>(type: "text", nullable: true),
                    Industrie = table.Column<string>(type: "text", nullable: true),
                    SiteWeb = table.Column<string>(type: "text", nullable: true),
                    Adresse = table.Column<string>(type: "text", nullable: true),
                    Ville = table.Column<string>(type: "text", nullable: true),
                    Pays = table.Column<string>(type: "text", nullable: true),
                    CodePostal = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    NomResponsable = table.Column<string>(type: "text", nullable: true),
                    PrenomResponsable = table.Column<string>(type: "text", nullable: true),
                    EmailResponsable = table.Column<string>(type: "text", nullable: true),
                    TelephoneResponsable = table.Column<string>(type: "text", nullable: true),
                    CreeLe = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                name: "IX_EntrepriseParSA_EntrepriseId",
                table: "EntrepriseParSA",
                column: "EntrepriseId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntrepriseParSA");

            migrationBuilder.AddColumn<string>(
                name: "Adresse",
                table: "InscriptionsEntreprises",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodePostal",
                table: "InscriptionsEntreprises",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "InscriptionsEntreprises",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Domaine",
                table: "InscriptionsEntreprises",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Industrie",
                table: "InscriptionsEntreprises",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pays",
                table: "InscriptionsEntreprises",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SiteWeb",
                table: "InscriptionsEntreprises",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ville",
                table: "InscriptionsEntreprises",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Adresse",
                table: "Entreprises",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodePostal",
                table: "Entreprises",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Entreprises",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Domaine",
                table: "Entreprises",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Industrie",
                table: "Entreprises",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pays",
                table: "Entreprises",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SiteWeb",
                table: "Entreprises",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ville",
                table: "Entreprises",
                type: "text",
                nullable: true);
        }
    }
}
