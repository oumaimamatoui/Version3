using System;
using System.Collections.Generic;

namespace NeoEvaluation.API.DTOs
{
    public class EnterpriseRecommendationsDto
    {
        public int AnomaliesCount { get; set; }
        public int DraftCampaignsCount { get; set; }
        public bool WeeklyReportAvailable { get; set; } = true;
        public List<CampaignAnomalyDto> Anomalies { get; set; } = new();
        public WeeklyPerformanceReportDto WeeklyReport { get; set; } = new();
        public List<DraftCampaignSetupDto> DraftCampaigns { get; set; } = new();
    }

    public class CampaignAnomalyDto
    {
        public Guid EvaluationId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public string CampaignName { get; set; } = string.Empty;
        public int InfractionsCount { get; set; }
        public int NbReprises { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Severity { get; set; } = "High";
    }

    public class WeeklyPerformanceReportDto
    {
        public int CompletedEvaluationsThisWeek { get; set; }
        public int TotalInvitationsThisWeek { get; set; }
        public float AverageScore { get; set; }
        public float AverageCompletionTimeMinutes { get; set; }
        public float CompletionRate { get; set; }
        public List<CampaignPerformanceDto> CampaignPerformances { get; set; } = new();
    }

    public class CampaignPerformanceDto
    {
        public string CampaignName { get; set; } = string.Empty;
        public int CandidatesCount { get; set; }
        public float AverageScore { get; set; }
    }

    public class DraftCampaignSetupDto
    {
        public Guid CampaignId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int QuestionsCount { get; set; }
        public bool HasQuestions { get; set; }
        public bool HasDescription { get; set; }
        public bool HasDuration { get; set; }
        public bool HasMaxCandidates { get; set; }
        public int CompletionPercentage { get; set; }
    }
}
