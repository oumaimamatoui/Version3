using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoEvaluation.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganisationDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "Pays",
                table: "InscriptionsEntreprises",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Secteur",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "Pays",
                table: "InscriptionsEntreprises");

            migrationBuilder.DropColumn(
                name: "Secteur",
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
                name: "Pays",
                table: "Entreprises");

            migrationBuilder.DropColumn(
                name: "SiteWeb",
                table: "Entreprises");

            migrationBuilder.DropColumn(
                name: "Ville",
                table: "Entreprises");
        }
    }
}
