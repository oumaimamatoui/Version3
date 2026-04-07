using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoEvaluation.API.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Utilisateurs_Roles_RoleId",
                table: "Utilisateurs");

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "Utilisateurs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MatriculeFiscale",
                table: "Entreprises",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Utilisateurs_Roles_RoleId",
                table: "Utilisateurs",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Utilisateurs_Roles_RoleId",
                table: "Utilisateurs");

            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "Utilisateurs");

            migrationBuilder.DropColumn(
                name: "MatriculeFiscale",
                table: "Entreprises");

            migrationBuilder.AddForeignKey(
                name: "FK_Utilisateurs_Roles_RoleId",
                table: "Utilisateurs",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
