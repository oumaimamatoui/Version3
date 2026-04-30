using System.Text.Json.Serialization;

namespace NeoEvaluation.API.DTOs
{
    public class SuperAdminStatsDto
    {
        [JsonPropertyName("totalEntreprises")]
        public int TotalEntreprises { get; set; }

        [JsonPropertyName("totalUtilisateurs")]
        public int TotalUtilisateurs { get; set; }

        [JsonPropertyName("demandesEnAttente")]
        public int DemandesEnAttente { get; set; }

        [JsonPropertyName("totalTests")]
        public int TotalTests { get; set; }

        [JsonPropertyName("croissanceStats")]
        public List<MonthlyGrowthDto> CroissanceStats { get; set; } = new();

        [JsonPropertyName("isGoogleConnected")]
        public bool IsGoogleConnected { get; set; }

        [JsonPropertyName("connectedEmail")]
        public string? ConnectedEmail { get; set; }
    }

    public class MonthlyGrowthDto
    {
        [JsonPropertyName("mois")]
        public string Mois { get; set; } = string.Empty;
        
        [JsonPropertyName("count")]
        public int Count { get; set; }
    }

    public class InviteAdminDto
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class AdminCreateOrgDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("matriculeFiscale")]
        public string? MatriculeFiscale { get; set; }
        
        // Nouveaux champs Organisation
        [JsonPropertyName("domain")]
        public string? Domaine { get; set; }

        [JsonPropertyName("industry")]
        public string? Industrie { get; set; }

        [JsonPropertyName("website")]
        public string? SiteWeb { get; set; }

        [JsonPropertyName("city")]
        public string? Ville { get; set; }

        [JsonPropertyName("country")]
        public string? Pays { get; set; }

        [JsonPropertyName("zipCode")]
        public string? CodePostal { get; set; }

        [JsonPropertyName("address")]
        public string? Adresse { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        // Infos Admin
        [JsonPropertyName("adminEmail")]
        public string AdminEmail { get; set; } = string.Empty;

        [JsonPropertyName("adminFirstName")]
        public string AdminFirstName { get; set; } = string.Empty;

        [JsonPropertyName("adminLastName")]
        public string AdminLastName { get; set; } = string.Empty;

        [JsonPropertyName("adminPhone")]
        public string? AdminPhone { get; set; }
    }
}
