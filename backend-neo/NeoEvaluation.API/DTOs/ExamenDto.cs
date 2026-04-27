namespace NeoEvaluation.API.DTOs
{
    public class ExamSetupDto
    {
        public Guid EvaluationId { get; set; }
        public string CampagneNom { get; set; } = string.Empty;
        public string Statut { get; set; } = string.Empty;
        public int TempsLimite { get; set; }
        public int CurrentIndex { get; set; }
        public List<QuestionItemDto> Questions { get; set; } = new();
    }

    public class QuestionItemDto
    {
        public Guid Id { get; set; }
        public string Enonce { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new();
        public float Points { get; set; }
    }

    public class ReponseDto
    {
        public Guid EvaluationId { get; set; }
        public Guid QuestionId { get; set; }
        public string Valeur { get; set; } = string.Empty; // Stocke index, texte ou nom fichier
        public int TempsSecondes { get; set; }
    }
}