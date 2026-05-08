using NeoEvaluation.API.Models;
using System.ComponentModel.DataAnnotations;

namespace NeoEvaluation.API.DTOs
{
    public class QuestionCreateDto
    {
        [Required]
        public string Enonce { get; set; } = string.Empty;
        
        public TypeQuestion Type { get; set; } = TypeQuestion.QCM;
        public NiveauComplexite Niveau { get; set; } = NiveauComplexite.INTERMEDIAIRE;
        public int Points { get; set; } = 1;
        public int? DureeSecondes { get; set; } 
        
        public string? Theme { get; set; }
        public string? SousTheme { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("choix")]
        public List<string> Choix { get; set; } = new List<string>();
        [System.Text.Json.Serialization.JsonPropertyName("bonneReponse")]
        public string BonneReponse { get; set; } = string.Empty;
        [System.Text.Json.Serialization.JsonPropertyName("prerequis")]
        public List<string> Prerequis { get; set; } = new List<string>();

        // Liaison (Many-to-Many)
        public Guid? QuestionnaireId { get; set; }
        
        // Propriétés de la liaison
        public int Ordre { get; set; } = 1;
        public float Ponderation { get; set; } = 1.0f;
        public bool EstObligatoire { get; set; } = true;
    }
}