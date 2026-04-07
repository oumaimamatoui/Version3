using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeoEvaluation.API.Models
{
    public class Campagne 
    {
        [Key] 
        public Guid? Id { get; set; } // Nullable est correct ici

        [Required(ErrorMessage = "Le nom est obligatoire")]
        public string Nom { get; set; } = string.Empty;

        public string? Description { get; set; }
        public string? Categorie { get; set; }
        public string? NiveauDifficulte { get; set; } = "Mixed Levels";

        public int DureeMinutes { get; set; } = 60;
        public int ScorePassage { get; set; } = 70;
        public int NbTentativesMax { get; set; } = 1;

        public DateTime DateDebut { get; set; } = DateTime.UtcNow;
        public DateTime DateFin { get; set; } = DateTime.UtcNow.AddDays(30);
        
        public int Statut { get; set; }
        public int MaxCandidats { get; set; }

        public Guid? QuestionnaireId { get; set; }
        public Questionnaire? Questionnaire { get; set; }
        
        [Required]
        public Guid EntrepriseId { get; set; }
        public virtual ICollection<Candidature> Candidatures { get; set; } = new List<Candidature>();
    }
}