using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoEvaluation.API.Migrations
{
    /// <inheritdoc />
    public partial class MakeQuestionnaireIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Questionnaires_QuestionnaireId",
                table: "Questions");

            migrationBuilder.AlterColumn<Guid>(
                name: "QuestionnaireId",
                table: "Questions",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Questionnaires_QuestionnaireId",
                table: "Questions",
                column: "QuestionnaireId",
                principalTable: "Questionnaires",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Questionnaires_QuestionnaireId",
                table: "Questions");

            migrationBuilder.AlterColumn<Guid>(
                name: "QuestionnaireId",
                table: "Questions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Questionnaires_QuestionnaireId",
                table: "Questions",
                column: "QuestionnaireId",
                principalTable: "Questionnaires",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
