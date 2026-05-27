namespace NeoEvaluation.API.Dtos
{
    public class StaffInvitationDto
    {
        public string Email { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public string NomFamille { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public Guid? EntrepriseId { get; set; }
    }
}
