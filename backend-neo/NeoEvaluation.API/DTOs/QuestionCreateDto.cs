// DTOs/QuestionCreateDto.cs
using System;
using System.Collections.Generic;

namespace NeoEvaluation.API.DTOs
{
    public class QuestionCreateDto
    {
        public Guid? Id { get; set; }
        public string Texte { get; set; } = string.Empty;
        public double Poids { get; set; }
        public int Type { get; set; }
        public Guid? QuestionnaireId { get; set; } // Nullable هنا أيضاً
        public List<object>? Reponses { get; set; } 
        public string? BonneReponse { get; set; } 
    }
}