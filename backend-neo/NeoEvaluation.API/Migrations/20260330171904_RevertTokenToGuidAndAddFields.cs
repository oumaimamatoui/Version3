using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoEvaluation.API.Migrations
{
    /// <inheritdoc />
    public partial class RevertTokenToGuidAndAddFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TokensActivation",
                table: "TokensActivation");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdInscription",
                table: "TokensActivation",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "TokensActivation",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreation",
                table: "TokensActivation",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "TokensActivation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UtilisateurId",
                table: "TokensActivation",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TokensActivation",
                table: "TokensActivation",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TokensActivation",
                table: "TokensActivation");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "TokensActivation");

            migrationBuilder.DropColumn(
                name: "DateCreation",
                table: "TokensActivation");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "TokensActivation");

            migrationBuilder.DropColumn(
                name: "UtilisateurId",
                table: "TokensActivation");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdInscription",
                table: "TokensActivation",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TokensActivation",
                table: "TokensActivation",
                column: "Token");
        }
    }
}
