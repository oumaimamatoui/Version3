using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoEvaluation.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreateurId",
                table: "Questions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreateurId",
                table: "Questionnaires",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CandidatId",
                table: "Evaluations",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PlanificateurId",
                table: "Campagnes",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Questions_CreateurId",
                table: "Questions",
                column: "CreateurId");

            migrationBuilder.CreateIndex(
                name: "IX_Questionnaires_CreateurId",
                table: "Questionnaires",
                column: "CreateurId");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_CandidatId",
                table: "Evaluations",
                column: "CandidatId");

            migrationBuilder.CreateIndex(
                name: "IX_Campagnes_PlanificateurId",
                table: "Campagnes",
                column: "PlanificateurId");

            migrationBuilder.AddForeignKey(
                name: "FK_Campagnes_Utilisateurs_PlanificateurId",
                table: "Campagnes",
                column: "PlanificateurId",
                principalTable: "Utilisateurs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Evaluations_Utilisateurs_CandidatId",
                table: "Evaluations",
                column: "CandidatId",
                principalTable: "Utilisateurs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Questionnaires_Utilisateurs_CreateurId",
                table: "Questionnaires",
                column: "CreateurId",
                principalTable: "Utilisateurs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Utilisateurs_CreateurId",
                table: "Questions",
                column: "CreateurId",
                principalTable: "Utilisateurs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Campagnes_Utilisateurs_PlanificateurId",
                table: "Campagnes");

            migrationBuilder.DropForeignKey(
                name: "FK_Evaluations_Utilisateurs_CandidatId",
                table: "Evaluations");

            migrationBuilder.DropForeignKey(
                name: "FK_Questionnaires_Utilisateurs_CreateurId",
                table: "Questionnaires");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Utilisateurs_CreateurId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_CreateurId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questionnaires_CreateurId",
                table: "Questionnaires");

            migrationBuilder.DropIndex(
                name: "IX_Evaluations_CandidatId",
                table: "Evaluations");

            migrationBuilder.DropIndex(
                name: "IX_Campagnes_PlanificateurId",
                table: "Campagnes");

            migrationBuilder.DropColumn(
                name: "CreateurId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "CreateurId",
                table: "Questionnaires");

            migrationBuilder.DropColumn(
                name: "CandidatId",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "PlanificateurId",
                table: "Campagnes");
        }
    }
}
