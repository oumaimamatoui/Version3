public class ReponseDto
{
    public Guid EvaluationId { get; set; }
    public Guid QuestionId { get; set; }
    public string Valeur { get; set; } = string.Empty;
    public int? InfractionsDetected { get; set; } // New field
}