using System;
using System.ComponentModel.DataAnnotations;

namespace NeoEvaluation.API.Models
{
    public class Reponse
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Valeur { get; set; } = string.Empty;

        public bool EstCorrecte { get; set; } = false;
        public int PointsObtenus { get; set; } = 0;
        public DateTime SoumisLe { get; set; } = DateTime.UtcNow;
        public int TempsSecondes { get; set; } = 0;

        public Guid QuestionId { get; set; }
        public Question Question { get; set; } = null!;

        public Guid EvaluationId { get; set; }
        public Evaluation Evaluation { get; set; } = null!;
    }
}