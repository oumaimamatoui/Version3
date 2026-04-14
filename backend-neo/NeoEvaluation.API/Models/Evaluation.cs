using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NeoEvaluation.API.Models
{
    public class Evaluation
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public float ScoreTotal { get; set; }
        public float ScorePourcentage { get; set; }
        public StatutPassage Statut { get; set; } = StatutPassage.NON_COMMENCE;
        public int NbReprises { get; set; } = 0;

        // Stocké en JSON: {"theme1": 85.5, "theme2": 72.0}
        public Dictionary<string, float> ScoresParTheme { get; set; } = new Dictionary<string, float>();

        public Guid CandidatureId { get; set; }
        public Candidature Candidature { get; set; } = null!;

        // Relations
        public ICollection<Reponse> Reponses { get; set; } = new List<Reponse>();
        public Rapport? Rapport { get; set; }

        // Relation: Un utilisateur effectue plusieurs passages
        public Guid? CandidatId { get; set; }
        public Utilisateur? Candidat { get; set; }
    }
}