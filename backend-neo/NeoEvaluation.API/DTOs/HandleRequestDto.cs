namespace NeoEvaluation.API.DTOs
{
    public class HandleRequestDto
    {
        public Guid RequestId { get; set; }
        public bool Accept { get; set; }
        public string? RejectionReason { get; set; }
    }
}
