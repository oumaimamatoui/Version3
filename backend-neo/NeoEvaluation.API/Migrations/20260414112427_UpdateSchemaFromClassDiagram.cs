using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoEvaluation.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSchemaFromClassDiagram : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Campagnes_Questionnaires_QuestionnaireId",
                table: "Campagnes");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Questionnaires_QuestionnaireId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_QuestionnaireId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Campagnes_QuestionnaireId",
                table: "Campagnes");

            migrationBuilder.DropColumn(
                name: "ScoreIA",
                table: "Reponses");

            migrationBuilder.DropColumn(
                name: "Poids",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "QuestionnaireId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "EstActif",
                table: "Entreprises");

            migrationBuilder.DropColumn(
                name: "Categorie",
                table: "Campagnes");

            migrationBuilder.DropColumn(
                name: "DureeMinutes",
                table: "Campagnes");

            migrationBuilder.DropColumn(
                name: "MaxCandidats",
                table: "Campagnes");

            migrationBuilder.DropColumn(
                name: "NbTentativesMax",
                table: "Campagnes");

            migrationBuilder.DropColumn(
                name: "NiveauDifficulte",
                table: "Campagnes");

            migrationBuilder.DropColumn(
                name: "QuestionnaireId",
                table: "Campagnes");

            migrationBuilder.DropColumn(
                name: "ScorePassage",
                table: "Campagnes");

            migrationBuilder.RenameColumn(
                name: "SoumiseLe",
                table: "Reponses",
                newName: "SoumisLe");

            migrationBuilder.RenameColumn(
                name: "Texte",
                table: "Questions",
                newName: "Prerequis");

            migrationBuilder.RenameColumn(
                name: "LimiteTemps",
                table: "Evaluations",
                newName: "NbReprises");

            migrationBuilder.AddColumn<DateTime>(
                name: "DerniereConnexion",
                table: "Utilisateurs",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EstCorrecte",
                table: "Reponses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PointsObtenus",
                table: "Reponses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TempsSecondes",
                table: "Reponses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Choix",
                table: "Questions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreeLe",
                table: "Questions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Enonce",
                table: "Questions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Niveau",
                table: "Questions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Points",
                table: "Questions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SousTheme",
                table: "Questions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Theme",
                table: "Questions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AntitricheActif",
                table: "Questionnaires",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DebutDispo",
                table: "Questionnaires",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DureeMinutes",
                table: "Questionnaires",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "EstPublie",
                table: "Questionnaires",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FinDispo",
                table: "Questionnaires",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxReprises",
                table: "Questionnaires",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "RandomiserQuestions",
                table: "Questionnaires",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ReprisesAutorisees",
                table: "Questionnaires",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateDebut",
                table: "Evaluations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateFin",
                table: "Evaluations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "ScorePourcentage",
                table: "Evaluations",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "ScoreTotal",
                table: "Evaluations",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "ScoresParTheme",
                table: "Evaluations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "AbonnementFin",
                table: "Entreprises",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Langue",
                table: "Entreprises",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Secteur",
                table: "Entreprises",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreeLe",
                table: "Campagnes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "CampagneQuestionnaires",
                columns: table => new
                {
                    CampagneId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionnaireId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampagneQuestionnaires", x => new { x.CampagneId, x.QuestionnaireId });
                    table.ForeignKey(
                        name: "FK_CampagneQuestionnaires_Campagnes_CampagneId",
                        column: x => x.CampagneId,
                        principalTable: "Campagnes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CampagneQuestionnaires_Questionnaires_QuestionnaireId",
                        column: x => x.QuestionnaireId,
                        principalTable: "Questionnaires",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionnaireQuestions",
                columns: table => new
                {
                    QuestionnaireId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Ordre = table.Column<int>(type: "integer", nullable: false),
                    Ponderation = table.Column<float>(type: "real", nullable: false),
                    EstObligatoire = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionnaireQuestions", x => new { x.QuestionnaireId, x.QuestionId });
                    table.ForeignKey(
                        name: "FK_QuestionnaireQuestions_Questionnaires_QuestionnaireId",
                        column: x => x.QuestionnaireId,
                        principalTable: "Questionnaires",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionnaireQuestions_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rapports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    GenereLe = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ScoresParTheme = table.Column<string>(type: "text", nullable: false),
                    ScoresParNiveau = table.Column<string>(type: "text", nullable: false),
                    TauxReussite = table.Column<float>(type: "real", nullable: false),
                    RecommandationsIA = table.Column<string>(type: "text", nullable: true),
                    CheminPDF = table.Column<string>(type: "text", nullable: true),
                    EstEnvoye = table.Column<bool>(type: "boolean", nullable: false),
                    EvaluationId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rapports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rapports_Evaluations_EvaluationId",
                        column: x => x.EvaluationId,
                        principalTable: "Evaluations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CampagneQuestionnaires_QuestionnaireId",
                table: "CampagneQuestionnaires",
                column: "QuestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireQuestions_QuestionId",
                table: "QuestionnaireQuestions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Rapports_EvaluationId",
                table: "Rapports",
                column: "EvaluationId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CampagneQuestionnaires");

            migrationBuilder.DropTable(
                name: "QuestionnaireQuestions");

            migrationBuilder.DropTable(
                name: "Rapports");

            migrationBuilder.DropColumn(
                name: "DerniereConnexion",
                table: "Utilisateurs");

            migrationBuilder.DropColumn(
                name: "EstCorrecte",
                table: "Reponses");

            migrationBuilder.DropColumn(
                name: "PointsObtenus",
                table: "Reponses");

            migrationBuilder.DropColumn(
                name: "TempsSecondes",
                table: "Reponses");

            migrationBuilder.DropColumn(
                name: "Choix",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "CreeLe",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Enonce",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Niveau",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Points",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "SousTheme",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Theme",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "AntitricheActif",
                table: "Questionnaires");

            migrationBuilder.DropColumn(
                name: "DebutDispo",
                table: "Questionnaires");

            migrationBuilder.DropColumn(
                name: "DureeMinutes",
                table: "Questionnaires");

            migrationBuilder.DropColumn(
                name: "EstPublie",
                table: "Questionnaires");

            migrationBuilder.DropColumn(
                name: "FinDispo",
                table: "Questionnaires");

            migrationBuilder.DropColumn(
                name: "MaxReprises",
                table: "Questionnaires");

            migrationBuilder.DropColumn(
                name: "RandomiserQuestions",
                table: "Questionnaires");

            migrationBuilder.DropColumn(
                name: "ReprisesAutorisees",
                table: "Questionnaires");

            migrationBuilder.DropColumn(
                name: "DateDebut",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "DateFin",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "ScorePourcentage",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "ScoreTotal",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "ScoresParTheme",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "AbonnementFin",
                table: "Entreprises");

            migrationBuilder.DropColumn(
                name: "Langue",
                table: "Entreprises");

            migrationBuilder.DropColumn(
                name: "Secteur",
                table: "Entreprises");

            migrationBuilder.DropColumn(
                name: "CreeLe",
                table: "Campagnes");

            migrationBuilder.RenameColumn(
                name: "SoumisLe",
                table: "Reponses",
                newName: "SoumiseLe");

            migrationBuilder.RenameColumn(
                name: "Prerequis",
                table: "Questions",
                newName: "Texte");

            migrationBuilder.RenameColumn(
                name: "NbReprises",
                table: "Evaluations",
                newName: "LimiteTemps");

            migrationBuilder.AddColumn<double>(
                name: "ScoreIA",
                table: "Reponses",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Poids",
                table: "Questions",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<Guid>(
                name: "QuestionnaireId",
                table: "Questions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Score",
                table: "Evaluations",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "EstActif",
                table: "Entreprises",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Categorie",
                table: "Campagnes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DureeMinutes",
                table: "Campagnes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxCandidats",
                table: "Campagnes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NbTentativesMax",
                table: "Campagnes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NiveauDifficulte",
                table: "Campagnes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "QuestionnaireId",
                table: "Campagnes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ScorePassage",
                table: "Campagnes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Questions_QuestionnaireId",
                table: "Questions",
                column: "QuestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_Campagnes_QuestionnaireId",
                table: "Campagnes",
                column: "QuestionnaireId");

            migrationBuilder.AddForeignKey(
                name: "FK_Campagnes_Questionnaires_QuestionnaireId",
                table: "Campagnes",
                column: "QuestionnaireId",
                principalTable: "Questionnaires",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Questionnaires_QuestionnaireId",
                table: "Questions",
                column: "QuestionnaireId",
                principalTable: "Questionnaires",
                principalColumn: "Id");
        }
    }
}
