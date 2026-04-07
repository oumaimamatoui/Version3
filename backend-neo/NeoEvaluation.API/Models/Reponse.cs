using System;
using System.ComponentModel.DataAnnotations;

namespace NeoEvaluation.API.Models
{
    public class Reponse
    {
        [Key] public Guid Id { get; set; } = Guid.NewGuid();
        [Required] public string Valeur { get; set; } = string.Empty;
        public double ScoreIA { get; set; }
        public DateTime SoumiseLe { get; set; } = DateTime.UtcNow;

        public Guid QuestionId { get; set; }
        public Question Question { get; set; } = null!;

        public Guid EvaluationId { get; set; }
        public Evaluation Evaluation { get; set; } = null!;
    }
}