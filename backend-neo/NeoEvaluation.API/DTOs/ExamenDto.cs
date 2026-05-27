namespace NeoEvaluation.API.DTOs
{
    /// <summary>
    /// DTO pour l'endpoint POST /api/Examen/notify-result
    /// </summary>
    public class NotifyResultDto
    {
        public Guid   EvaluationId   { get; set; }
        public int    Pourcentage     { get; set; }
        public bool   Passed          { get; set; }
        public int    IntegrityScore  { get; set; }
    }

    /// <summary>
    /// DTO pour l'endpoint POST /api/Examen/save-response
    /// </summary>
    public class ReponseDto
    {
        public Guid   EvaluationId { get; set; }
        public Guid   QuestionId   { get; set; }
        public string? Valeur      { get; set; }
    }
}