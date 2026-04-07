namespace NeoEvaluation.API.Dtos
{
    public class InvitationRequestDto
    {
        public Guid CampagneId { get; set; }
        public List<string> Emails { get; set; } = new();
    }
}