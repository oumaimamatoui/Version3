using NeoEvaluation.API.Models;

namespace NeoEvaluation.API.DTOs
{
    public class QuestionCreateDto
    {
        public string Enonce { get; set; } = string.Empty;

        // ✅ Enums directement — le deserializer JSON accepte les int (0,1,2...)
        public TypeQuestion Type { get; set; } = TypeQuestion.QCM;
        public NiveauComplexite Niveau { get; set; } = NiveauComplexite.INTERMEDIAIRE;

        public int Points { get; set; } = 1;
        public int? DureeSecondes { get; set; }

        public string? Theme { get; set; }
        public string? SousTheme { get; set; }

        // ✅ AJOUTÉ — champ langue manquant dans l'original
        public string? Langue { get; set; } = "fr";

        public List<string>? Choix { get; set; }
        public string? BonneReponse { get; set; }
        public Guid? QuestionnaireId { get; set; }
    }
}