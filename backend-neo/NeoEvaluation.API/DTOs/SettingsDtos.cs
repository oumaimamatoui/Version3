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
    }

    public class BrandingUpdateDto
    {
        public string CompanyName { get; set; } = string.Empty;
        public string Color { get; set; } = "#6366f1";
    }
}