using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoEvaluation.API.Migrations
{
    /// <inheritdoc />
    public partial class AddOrgMetadata : Migration
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
                name: "PrenomResponsable",
                table: "InscriptionsEntreprises",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SiteWeb",
                table: "InscriptionsEntreprises",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelephoneResponsable",
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
                name: "Industrie",
                table: "InscriptionsEntreprises");

            migrationBuilder.DropColumn(
                name: "Pays",
                table: "InscriptionsEntreprises");

            migrationBuilder.DropColumn(
                name: "PrenomResponsable",
                table: "InscriptionsEntreprises");

            migrationBuilder.DropColumn(
                name: "SiteWeb",
                table: "InscriptionsEntreprises");

            migrationBuilder.DropColumn(
                name: "TelephoneResponsable",
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
        }
    }
}
