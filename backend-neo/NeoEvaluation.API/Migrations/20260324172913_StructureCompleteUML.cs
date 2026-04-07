using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoEvaluation.API.Migrations
{
    /// <inheritdoc />
    public partial class StructureCompleteUML : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Entreprises",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nom = table.Column<string>(type: "text", nullable: false),
                    Plan = table.Column<string>(type: "text", nullable: false),
                    CreeLe = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EstActif = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entreprises", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InscriptionsEntreprises",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NomEntreprise = table.Column<string>(type: "text", nullable: false),
                    MatriculeFiscale = table.Column<string>(type: "text", nullable: true),
                    NomResponsable = table.Column<string>(type: "text", nullable: false),
                    EmailResponsable = table.Column<string>(type: "text", nullable: false),
                    Statut = table.Column<int>(type: "integer", nullable: false),
                    CreeLe = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InscriptionsEntreprises", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Invitations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    Statut = table.Column<string>(type: "text", nullable: false),
                    DateEnvoi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CampagneId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invitations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Questionnaires",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Titre = table.Column<string>(type: "text", nullable: false),
                    Difficulte = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questionnaires", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nom = table.Column<string>(type: "text", nullable: false),
                    Permissions = table.Column<List<string>>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TokensActivation",
                columns: table => new
                {
                    Token = table.Column<Guid>(type: "uuid", nullable: false),
                    IdInscription = table.Column<Guid>(type: "uuid", nullable: false),
                    DateExpiration = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Utilise = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokensActivation", x => x.Token);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Texte = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Poids = table.Column<double>(type: "double precision", nullable: false),
                    BonneReponse = table.Column<string>(type: "text", nullable: false),
                    QuestionnaireId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_Questionnaires_QuestionnaireId",
                        column: x => x.QuestionnaireId,
                        principalTable: "Questionnaires",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Utilisateurs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    MotDePasseHash = table.Column<string>(type: "text", nullable: false),
                    Nom = table.Column<string>(type: "text", nullable: false),
                    Prenom = table.Column<string>(type: "text", nullable: false),
                    RoleNom = table.Column<string>(type: "text", nullable: false),
                    EstActif = table.Column<bool>(type: "boolean", nullable: false),
                    NomEntreprise = table.Column<string>(type: "text", nullable: true),
                    CreeLe = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserType = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    Telephone = table.Column<string>(type: "text", nullable: true),
                    Adresse = table.Column<string>(type: "text", nullable: true),
                    ProfilComplet = table.Column<bool>(type: "boolean", nullable: true),
                    IdEmploye = table.Column<string>(type: "text", nullable: true),
                    Departement = table.Column<string>(type: "text", nullable: true),
                    EntrepriseId = table.Column<Guid>(type: "uuid", nullable: true),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: true),
                    Privileges = table.Column<List<string>>(type: "text[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilisateurs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Utilisateurs_Entreprises_EntrepriseId",
                        column: x => x.EntrepriseId,
                        principalTable: "Entreprises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Utilisateurs_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Campagnes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nom = table.Column<string>(type: "text", nullable: false),
                    DateDebut = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateFin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Statut = table.Column<int>(type: "integer", nullable: false),
                    EntrepriseId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionnaireId = table.Column<Guid>(type: "uuid", nullable: true),
                    PersonnelId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campagnes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Campagnes_Entreprises_EntrepriseId",
                        column: x => x.EntrepriseId,
                        principalTable: "Entreprises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Campagnes_Questionnaires_QuestionnaireId",
                        column: x => x.QuestionnaireId,
                        principalTable: "Questionnaires",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Campagnes_Utilisateurs_PersonnelId",
                        column: x => x.PersonnelId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DocumentsCandidats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NomFichier = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    EstVerifie = table.Column<bool>(type: "boolean", nullable: false),
                    CandidatId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentsCandidats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentsCandidats_Utilisateurs_CandidatId",
                        column: x => x.CandidatId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Candidatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Statut = table.Column<int>(type: "integer", nullable: false),
                    PostuleLe = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CandidatId = table.Column<Guid>(type: "uuid", nullable: false),
                    CampagneId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Candidatures_Campagnes_CampagneId",
                        column: x => x.CampagneId,
                        principalTable: "Campagnes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Candidatures_Utilisateurs_CandidatId",
                        column: x => x.CandidatId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NomFichier = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    TelechargeLe = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CampagneId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_Campagnes_CampagneId",
                        column: x => x.CampagneId,
                        principalTable: "Campagnes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateTable(
                name: "Evaluations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Score = table.Column<double>(type: "double precision", nullable: false),
                    Statut = table.Column<int>(type: "integer", nullable: false),
                    LimiteTemps = table.Column<int>(type: "integer", nullable: false),
                    CandidatureId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Evaluations_Candidatures_CandidatureId",
                        column: x => x.CandidatureId,
                        principalTable: "Candidatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reponses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Valeur = table.Column<string>(type: "text", nullable: false),
                    ScoreIA = table.Column<double>(type: "double precision", nullable: false),
                    SoumiseLe = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    EvaluationId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reponses_Evaluations_EvaluationId",
                        column: x => x.EvaluationId,
                        principalTable: "Evaluations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reponses_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Campagnes_EntrepriseId",
                table: "Campagnes",
                column: "EntrepriseId");

            migrationBuilder.CreateIndex(
                name: "IX_Campagnes_PersonnelId",
                table: "Campagnes",
                column: "PersonnelId");

            migrationBuilder.CreateIndex(
                name: "IX_Campagnes_QuestionnaireId",
                table: "Campagnes",
                column: "QuestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_Candidatures_CampagneId",
                table: "Candidatures",
                column: "CampagneId");

            migrationBuilder.CreateIndex(
                name: "IX_Candidatures_CandidatId",
                table: "Candidatures",
                column: "CandidatId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CampagneId",
                table: "Documents",
                column: "CampagneId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentsCandidats_CandidatId",
                table: "DocumentsCandidats",
                column: "CandidatId");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_CandidatureId",
                table: "Evaluations",
                column: "CandidatureId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Plannings_CampagneId",
                table: "Plannings",
                column: "CampagneId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_QuestionnaireId",
                table: "Questions",
                column: "QuestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_Reponses_EvaluationId",
                table: "Reponses",
                column: "EvaluationId");

            migrationBuilder.CreateIndex(
                name: "IX_Reponses_QuestionId",
                table: "Reponses",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateurs_EntrepriseId",
                table: "Utilisateurs",
                column: "EntrepriseId");

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateurs_RoleId",
                table: "Utilisateurs",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "DocumentsCandidats");

            migrationBuilder.DropTable(
                name: "InscriptionsEntreprises");

            migrationBuilder.DropTable(
                name: "Invitations");

            migrationBuilder.DropTable(
                name: "Plannings");

            migrationBuilder.DropTable(
                name: "Reponses");

            migrationBuilder.DropTable(
                name: "TokensActivation");

            migrationBuilder.DropTable(
                name: "Evaluations");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Candidatures");

            migrationBuilder.DropTable(
                name: "Campagnes");

            migrationBuilder.DropTable(
                name: "Questionnaires");

            migrationBuilder.DropTable(
                name: "Utilisateurs");

            migrationBuilder.DropTable(
                name: "Entreprises");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
