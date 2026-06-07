using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoEvaluation.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStripeFieldsToInscriptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentStatus",
                table: "InscriptionsEntreprises",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "StripeSessionId",
                table: "InscriptionsEntreprises",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "InscriptionsEntreprises");

            migrationBuilder.DropColumn(
                name: "StripeSessionId",
                table: "InscriptionsEntreprises");
        }
    }
}
