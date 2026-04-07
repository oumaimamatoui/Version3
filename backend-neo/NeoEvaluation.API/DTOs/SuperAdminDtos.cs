namespace NeoEvaluation.API.DTOs
{
    public class SuperAdminStatsDto
    {
        public int TotalEntreprises { get; set; }
        public int TotalUtilisateurs { get; set; }
        public int DemandesEnAttente { get; set; }
        public int SessionsIARecentes { get; set; }
    }

    public class InviteAdminDto
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class AdminCreateOrgDto
    {
        public string Name { get; set; } = string.Empty;
        public string? MatriculeFiscale { get; set; }
        public string AdminEmail { get; set; } = string.Empty;
        public string AdminFirstName { get; set; } = string.Empty;
        public string AdminLastName { get; set; } = string.Empty;
    }
}
