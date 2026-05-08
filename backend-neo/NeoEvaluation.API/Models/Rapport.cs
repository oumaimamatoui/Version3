using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeoEvaluation.API.Models
{
    public class Rapport
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public TypeRapport Type { get; set; } = TypeRapport.PAR_CANDIDAT;
        public DateTime GenereLe { get; set; } = DateTime.UtcNow;

        // Stockés en JSON
        public Dictionary<string, float> ScoresParTheme { get; set; } = new Dictionary<string, float>();
        public Dictionary<string, float> ScoresParNiveau { get; set; } = new Dictionary<string, float>();

        public float TauxReussite { get; set; }
        public string? RecommandationsIA { get; set; }
        public string? CheminPDF { get; set; }
        public bool EstEnvoye { get; set; } = false;

        // Relation 1-1 avec Evaluation (Passage)
        public Guid EvaluationId { get; set; }
        public Evaluation Evaluation { get; set; } = null!;
    }
}
