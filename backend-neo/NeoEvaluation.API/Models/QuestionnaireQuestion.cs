using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeoEvaluation.API.Models
{
    public class QuestionnaireQuestion
    {
        public Guid QuestionnaireId { get; set; }
        public Questionnaire Questionnaire { get; set; } = null!;

        public Guid QuestionId { get; set; }
        public Question Question { get; set; } = null!;

        public int Ordre { get; set; } = 0;
        public float Ponderation { get; set; } = 1.0f;
        public bool EstObligatoire { get; set; } = true;
    }
}
