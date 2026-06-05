using System.Text.Json.Serialization;

namespace NeoEvaluation.API.DTOs
{
    public class SuperAdminStatsDto
    {
        [JsonPropertyName("totalEntreprises")]
        public int TotalEntreprises { get; set; }

        [JsonPropertyName("totalUtilisateurs")]
        public int TotalUtilisateurs { get; set; }

        [JsonPropertyName("activeCount")]
        public int ActiveCount { get; set; }

        [JsonPropertyName("inactiveCount")]
        public int InactiveCount { get; set; }

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

        [JsonPropertyName("startupCount")]
        public int StartupCount { get; set; }

        [JsonPropertyName("businessCount")]
        public int BusinessCount { get; set; }

        [JsonPropertyName("enterpriseCount")]
        public int EnterpriseCount { get; set; }

        [JsonPropertyName("totalRevenus")]
        public double TotalRevenus { get; set; }

        [JsonPropertyName("totalEntreprises7Days")]
        public int TotalEntreprises7Days { get; set; }

        [JsonPropertyName("totalUtilisateurs7Days")]
        public int TotalUtilisateurs7Days { get; set; }

        [JsonPropertyName("totalTests7Days")]
        public int TotalTests7Days { get; set; }

        [JsonPropertyName("totalRevenus7Days")]
        public double TotalRevenus7Days { get; set; }

        [JsonPropertyName("totalEntreprises30Days")]
        public int TotalEntreprises30Days { get; set; }

        [JsonPropertyName("totalUtilisateurs30Days")]
        public int TotalUtilisateurs30Days { get; set; }

        [JsonPropertyName("totalTests30Days")]
        public int TotalTests30Days { get; set; }

        [JsonPropertyName("totalRevenus30Days")]
        public double TotalRevenus30Days { get; set; }

        [JsonPropertyName("gratuitCount")]
        public int GratuitCount { get; set; }

        [JsonPropertyName("monthlyRevenues")]
        public List<double> MonthlyRevenues { get; set; } = new();

        [JsonPropertyName("recentTransactions")]
        public List<RecentTransactionDto> RecentTransactions { get; set; } = new();
    }

    public class RecentTransactionDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("plan")]
        public string Plan { get; set; } = string.Empty;

        [JsonPropertyName("date")]
        public string Date { get; set; } = string.Empty;

        [JsonPropertyName("price")]
        public double Price { get; set; }

        [JsonPropertyName("color")]
        public string Color { get; set; } = "#6366f1";
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
    }

    public class MailerDiagnosticsDto
    {
        [JsonPropertyName("isGoogleConnected")]
        public bool IsGoogleConnected { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("pendingInvitesCount")]
        public int PendingInvitesCount { get; set; }

        [JsonPropertyName("diagnosticsLogs")]
        public List<string> DiagnosticsLogs { get; set; } = new();
    }

    public class ExpiringSubscriptionDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("plan")]
        public string Plan { get; set; } = string.Empty;

        [JsonPropertyName("expirationDate")]
        public DateTime? ExpirationDate { get; set; }

        [JsonPropertyName("daysRemaining")]
        public int DaysRemaining { get; set; }

        [JsonPropertyName("adminEmail")]
        public string AdminEmail { get; set; } = string.Empty;

        [JsonPropertyName("adminName")]
        public string AdminName { get; set; } = string.Empty;
    }

    public class SecurityStatusDto
    {
        [JsonPropertyName("lastAuditDate")]
        public DateTime? LastAuditDate { get; set; }

        [JsonPropertyName("daysSinceLastAudit")]
        public int? DaysSinceLastAudit { get; set; }

        [JsonPropertyName("isAuditRecommended")]
        public bool IsAuditRecommended { get; set; }

        [JsonPropertyName("securityScore")]
        public int SecurityScore { get; set; }
    }

    public class SecurityScanResultDto
    {
        [JsonPropertyName("checkedItems")]
        public List<SecurityCheckItem> CheckedItems { get; set; } = new();

        [JsonPropertyName("securityScore")]
        public int SecurityScore { get; set; }

        [JsonPropertyName("weakPasswordsCount")]
        public int WeakPasswordsCount { get; set; }

        [JsonPropertyName("expiredTokensCleaned")]
        public int ExpiredTokensCleaned { get; set; }
    }

    public class SecurityCheckItem
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = "OK"; // "OK", "WARNING", "CRITICAL"

        [JsonPropertyName("details")]
        public string Details { get; set; } = string.Empty;
    }

    public class SuperAdminUpdateOrgDto
    {
        [JsonPropertyName("nom")]
        public string Nom { get; set; } = string.Empty;

        [JsonPropertyName("plan")]
        public string Plan { get; set; } = string.Empty;

        [JsonPropertyName("secteur")]
        public string? Secteur { get; set; }

        [JsonPropertyName("domaine")]
        public string? Domaine { get; set; }

        [JsonPropertyName("siteWeb")]
        public string? SiteWeb { get; set; }

        [JsonPropertyName("ville")]
        public string? Ville { get; set; }

        [JsonPropertyName("pays")]
        public string? Pays { get; set; }

        [JsonPropertyName("matriculeFiscale")]
        public string? MatriculeFiscale { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("estActif")]
        public bool EstActif { get; set; }
    }

    public class SystemHealthDto
    {
        [JsonPropertyName("cpu")]
        public double Cpu { get; set; }

        [JsonPropertyName("ram")]
        public double Ram { get; set; }

        [JsonPropertyName("disk")]
        public double Disk { get; set; }

        [JsonPropertyName("uptime")]
        public double Uptime { get; set; }

        [JsonPropertyName("os")]
        public string? Os { get; set; }
    }

    public class SuperAdminCampagnesPerformanceDto
    {
        [JsonPropertyName("campagnes")]
        public List<SuperAdminCampagneScoreDto> Campagnes { get; set; } = new();

        [JsonPropertyName("talentsDetectes")]
        public List<SuperAdminTalentDetecteDto> TalentsDetectes { get; set; } = new();
    }

    public class SuperAdminCampagneScoreDto
    {
        [JsonPropertyName("nom")]
        public string Nom { get; set; } = string.Empty;

        [JsonPropertyName("scoreMoyen")]
        public double ScoreMoyen { get; set; }

        [JsonPropertyName("nbEvaluations")]
        public int NbEvaluations { get; set; }
    }

    public class SuperAdminTalentDetecteDto
    {
        [JsonPropertyName("nomComplet")]
        public string NomComplet { get; set; } = string.Empty;

        [JsonPropertyName("score")]
        public double Score { get; set; }

        [JsonPropertyName("campagne")]
        public string Campagne { get; set; } = string.Empty;
    }

    public class MonthlyEvalStatsDto
    {
        [JsonPropertyName("label")]
        public string Label { get; set; } = string.Empty;

        [JsonPropertyName("sessions")]
        public int Sessions { get; set; }

        [JsonPropertyName("users")]
        public int Users { get; set; }

        [JsonPropertyName("score")]
        public double Score { get; set; }
    }
}