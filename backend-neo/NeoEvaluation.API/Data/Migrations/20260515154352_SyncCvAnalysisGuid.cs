using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoEvaluation.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class SyncCvAnalysisGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
{
    // 1. On change le type de Lang (ça, ça fonctionnait déjà)
    migrationBuilder.AlterColumn<string>(
        name: "Lang",
        table: "CvAnalyses",
        type: "character varying(10)",
        maxLength: 10,
        nullable: false,
        oldClrType: typeof(string),
        oldType: "text");

    // 2. AU LIEU DE "AlterColumn" pour CandidatId, on fait ceci :
    
    // Supprimer l'ancienne colonne 'int'
    migrationBuilder.DropColumn(
        name: "CandidatId",
        table: "CvAnalyses");

    // Recréer la colonne avec le type 'uuid'
    migrationBuilder.AddColumn<Guid>(
        name: "CandidatId",
        table: "CvAnalyses",
        type: "uuid",
        nullable: true);
}
    }
}
