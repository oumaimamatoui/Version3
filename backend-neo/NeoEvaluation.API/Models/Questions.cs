using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace NeoEvaluation.API.Models
{
    public class Question : IMultiTenant
    {
        public Guid? EntrepriseId { get; set; }
        
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Enonce { get; set; } = string.Empty;

        public TypeQuestion Type { get; set; } = TypeQuestion.QCM;
        public NiveauComplexite Niveau { get; set; } = NiveauComplexite.INTERMEDIAIRE;
        public int Points { get; set; } = 1;
        public int? DureeSecondes { get; set; } // Durée individuelle pour cette question
        [Column("Categorie")]
        public string? Theme { get; set; }

        public string? SousTheme { get; set; }

        // Stocké en JSON dans la DB
        public List<string> Choix { get; set; } = new List<string>();
        public string BonneReponse { get; set; } = string.Empty;
        public List<string> Prerequis { get; set; } = new List<string>();

        public DateTime CreeLe { get; set; } = DateTime.UtcNow;

        // Relation Many-to-Many via table de jointure
        [JsonIgnore]
        public ICollection<QuestionnaireQuestion> QuestionnaireQuestions { get; set; } = new List<QuestionnaireQuestion>();

        // Relation: Un utilisateur crée plusieurs questions
        public Guid? CreateurId { get; set; }
        public Utilisateur? Createur { get; set; }
    }
}