using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace NeoEvaluation.API.Models
{
    public class Question
    {
        [Key] 
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required] 
        public string Texte { get; set; } = string.Empty;

        public int Type { get; set; } // 0: QCM, 1: Texte, 2: Code, 3: Fichier

        public double Poids { get; set; }

        public string BonneReponse { get; set; } = string.Empty;

        [Required]
        public Guid QuestionnaireId { get; set; }

        [JsonIgnore] // Empêche les erreurs de validation et les cycles JSON
        [ForeignKey("QuestionnaireId")]
        public Questionnaire? Questionnaire { get; set; }
    }
}