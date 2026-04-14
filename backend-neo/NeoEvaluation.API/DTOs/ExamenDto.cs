namespace NeoEvaluation.API.DTOs
{
    public class ExamSetupDto
    {
        public Guid EvaluationId { get; set; }
        public string CampagneNom { get; set; } = string.Empty;
        public int TempsLimite { get; set; }
        public List<QuestionItemDto> Questions { get; set; } = new();
    }

    public class QuestionItemDto
    {
        public Guid Id { get; set; }
        public string Enonce { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new();
    }

    public class SyncDto
    {
        public Guid EvaluationId { get; set; }
        public int CurrentIndex { get; set; }
        public List<string> ReponsesChoisies { get; set; } = new();
        public int Integrite { get; set; }
    }
}