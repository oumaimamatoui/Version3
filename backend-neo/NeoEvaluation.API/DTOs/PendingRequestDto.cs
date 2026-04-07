namespace NeoEvaluation.API.DTOs
{
    public class PendingRequestDto
    {
        public Guid Id { get; set; }
        public string NomEntreprise { get; set; } = string.Empty;
        public string NomResponsable { get; set; } = string.Empty;
        public string EmailResponsable { get; set; } = string.Empty;
        public DateTime DateDemande { get; set; }
    }
}
