namespace NeoEvaluation.API.Dtos
{
    public class UserProfileDto
    {
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }
        public string? Bio { get; set; }
        public string? JoinDate { get; set; } // Formaté pour l'affichage
        public string ThemePreference { get; set; } = "light";
        public string? EntrepriseNom { get; set; }
        public string? SubscriptionPlan { get; set; }
        public string? SubscriptionDate { get; set; }
        public string? SubscriptionExpiry { get; set; }
    }
    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class BrandingDto
    {
        public string CompanyName { get; set; } = string.Empty;
        public string Color { get; set; } = "#6366f1";
        public string? LogoUrl { get; set; }

        // Champs Profil Entreprise
        public string? Domaine { get; set; }
        public string? Secteur { get; set; }
        public string? SiteWeb { get; set; }
        public string? Ville { get; set; }
        public string? Pays { get; set; }
        public string? CodePostal { get; set; }
        public string? Adresse { get; set; }
        public string? Description { get; set; }
        public string? MatriculeFiscale { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("isGoogleConnected")]
        public bool IsGoogleConnected { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("connectedEmail")]
        public string? ConnectedEmail { get; set; }
    }

    public class BrandingUpdateDto
    {
        public string CompanyName { get; set; } = string.Empty;
        public string Color { get; set; } = "#6366f1";
        
        public string? Domaine { get; set; }
        public string? Secteur { get; set; }
        public string? SiteWeb { get; set; }
        public string? Ville { get; set; }
        public string? Pays { get; set; }
        public string? CodePostal { get; set; }
        public string? Adresse { get; set; }
        public string? Description { get; set; }
        public string? MatriculeFiscale { get; set; }
    }
}